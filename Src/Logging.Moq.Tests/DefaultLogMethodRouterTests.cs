using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace su3dev.Logging.Moq.Tests
{
    public class DefaultLogMethodRouterTests
    {
        [Fact]
        public void Ctor_SetsTarget()
        {
            var sut = GetTestableSubject(out var targetMock).Object;

            sut.Target.Should().Be(targetMock.Object);
        }

        [Fact]
        public void InvokeLogMethod_CallsTarget()
        {
            var sut = GetTestableSubject(out var targetMock);

            _ = InvokeProtected(sut, "InvokeLogMethod",
                new object[] { "SomeMethod", new object[] { "someValue" } });
            
            targetMock.Verify(m => m.SomeMethod("someValue"), Times.Once);
        }

        [Theory]
        [InlineData(LogLevel.Information, "LogInformation")]
        [InlineData(LogLevel.Trace, "LogTrace")]
        [InlineData(LogLevel.Debug, "LogDebug")]
        [InlineData(LogLevel.Warning, "LogWarning")]
        [InlineData(LogLevel.Error, "LogError")]
        [InlineData(LogLevel.Critical, "LogCritical")]
        [InlineData(LogLevel.None, "")]
        public void GetLogMethodName_ReturnsCorrectLogMethodName(LogLevel logLevel, string expectedMethodName)
        {
            var sut = GetTestableSubject();

            var actual = InvokeProtected(sut, "GetLogMethodName", new object[] { logLevel });

            actual.Should().Be(expectedMethodName);
        }

        [Fact]
        public void GetLogMethodParameters_ReturnsObjectArray()
        {
            var sut = GetTestableSubject();

            var actual = InvokeProtected(sut, "GetLogMethodParameters", typeof(object), new[] { new EventId(0), new object(), null });

            actual.Should().BeAssignableTo<object[]>();
        }
        
        [Theory]
        //          event id, should contain it
        [InlineData(       1,              true)]
        [InlineData(       0,             false)]
        public void GetLogMethodParameters_AddsEventId_OnlyWhenEventIdIsNotDefault(int eventId, bool shouldContainEventId)
        {
            var sut = GetTestableSubject();

            var eventIdInstance = new EventId(eventId);
            var actual = InvokeProtected(sut, "GetLogMethodParameters", typeof(object), new[] { eventIdInstance, new object(), null });

            var actualArray = actual as object[];
            actualArray!.Contains(eventIdInstance).Should().Be(shouldContainEventId);
        }

        [Theory]
        //          add exception, should contain it
        [InlineData(         true,              true)]
        [InlineData(        false,             false)]
        public void GetLogMethodParameters_AddsException_OnlyWhenExceptionIsNotNull(bool addException, bool shouldContainException)
        {
            var sut = GetTestableSubject();

            var exception = new UnitTestForcedException();
            var actual = InvokeProtected(sut, "GetLogMethodParameters", typeof(object),
                new[] { new EventId(0), new object(), addException ? exception : null });

            var actualArray = actual as object[];
            actualArray!.Contains(exception).Should().Be(shouldContainException);
        }

        [Fact]
        public void GetLogMethodParameters_AddsStringWithNullText_WhenStateIsNotIEnumerable()
        {
            var sut = GetTestableSubject();

            var actual = InvokeProtected(sut, "GetLogMethodParameters", typeof(object),
                new[] { new EventId(0), new object(), null });

            var actualArray = actual as object[];
            actualArray.Should().Contain(Logger.NullOriginalFormatValue);
        }

        [Fact]
        public void GetLogMethodParameters_AddsStringWithNullText_WhenStateIsNotAnIEnumerableOfKeyValuePairs()
        {
            var sut = GetTestableSubject();

            var state = new List<string> { "one", "two" };
            var actual = InvokeProtected(sut, "GetLogMethodParameters", state.GetType(),
                new object?[] { new EventId(0), state, null });

            var actualArray = actual as object[];
            actualArray.Should().Contain(Logger.NullOriginalFormatValue);
        }

        [Fact]
        public void GetLogMethodParameters_AddsStringWithNullText_WhenStateDoesNotHaveAnOriginalFormatPair()
        {
            var sut = GetTestableSubject();

            var state = new List<KeyValuePair<string, object>> { new("one", "two") };
            var actual = InvokeProtected(sut, "GetLogMethodParameters", state.GetType(),
                new object?[] { new EventId(0), state, null });

            var actualArray = actual as object[];
            actualArray.Should().Contain(Logger.NullOriginalFormatValue);
        }

        [Fact]
        public void GetLogMethodParameters_AddsStringWithOriginalFormat()
        {
            var sut = GetTestableSubject();

            var expectedOriginalFormatValue = "some text";
            var state = new List<KeyValuePair<string, object>> { new(Logger.OriginalFormatKey, expectedOriginalFormatValue) };
            var actual = InvokeProtected(sut, "GetLogMethodParameters", state.GetType(),
                new object?[] { new EventId(0), state, null });

            var actualArray = actual as object[];
            actualArray.Should().Contain(expectedOriginalFormatValue);
        }

        [Theory]
        //          add args, should contain it
        [InlineData(    true,              true)]
        [InlineData(   false,             false)]
        public void GetLogMethodParameters_AddsArgs_OnlyWhenItIsNotEmpty(bool addArgs, bool shouldContainArgs)
        {
            var sut = GetTestableSubject();

            const string arg1Key = "arg-1-key";
            const string arg1Value = "arg-1-value";
            var state = new List<KeyValuePair<string, object>>();
            if (addArgs)
            {
                state.Add(new KeyValuePair<string, object>(arg1Key, arg1Value));
            }
            var parameters = InvokeProtected(sut, "GetLogMethodParameters", state.GetType(),
                new object?[] { new EventId(0), state, null });

            var parametersAsArray = parameters as object[];
            var argsElement = parametersAsArray!.Where(e => e.GetType() == typeof(object[])).ToList();
            argsElement.Any().Should().Be(shouldContainArgs);
        }

        [Fact]
        public void GetLogMethodParameters_AddsArgs_ButSkipsOriginalFormat()
        {
            var sut = GetTestableSubject();

            const string arg1Key = "arg-1-key";
            const string arg1Value = "arg-1-value";
            const string arg2Key = "arg-2-key";
            const string arg2Value = "arg-2-value";
            const string originalFormat = "some-text";
            var state = new List<KeyValuePair<string, object>>
            {
                new(Logger.OriginalFormatKey, originalFormat),
                new(arg1Key, arg1Value),
                new(arg2Key, arg2Value)
            };
            
            var parameters = InvokeProtected(sut, "GetLogMethodParameters", state.GetType(),
                new object?[] { new EventId(0), state, null });

            var parametersAsArray = parameters as object[];
            // the resulting array contains a sub-array with the args
            var args = parametersAsArray!.FirstOrDefault(e => e.GetType() == typeof(object[])).As<object[]>();
            args.Should().Contain(arg1Value, arg2Value);
            args.Should().NotContain(originalFormat);
        }
       
        [Fact]
        public void GetLogMethodParameters_AddsArgs_InTheCorrectOrder()
        {
            var sut = GetTestableSubject();

            var eventId = new EventId(10);
            
            const string arg1Key = "arg-1-key";
            const string arg1Value = "arg-1-value";
            const string originalFormat = "some-text";
            var state = new List<KeyValuePair<string, object>>
            {
                new(Logger.OriginalFormatKey, originalFormat),
                new(arg1Key, arg1Value),
            };

            var exception = new UnitTestForcedException();
            
            var parameters = InvokeProtected(sut, "GetLogMethodParameters", state.GetType(),
                new object?[] { eventId, state, exception });

            var parametersAsArray = parameters as object[];
            parametersAsArray.Should().BeEquivalentTo(new object[]
                {
                    eventId,
                    exception,
                    originalFormat,
                    new[] { arg1Value }
                }, options => options.WithStrictOrdering()
            );
        }

        [Fact]
        public void RouteCall_DoesNotInvokeLogMethod_WhenLogLevelIsNotRecognized()
        {
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;

            sut.RouteCall<object>(LogLevel.None, new EventId(0), new object(), null);
            
            sutMock.Protected().Verify("InvokeLogMethod", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<object[]>());
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void RouteCall_CallsGetLogMethodName_WithLogLevel(LogLevel logLevel)
        {
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;

            sut.RouteCall<object>(logLevel, new EventId(0), new object(), null);
            
            sutMock.Protected().Verify("GetLogMethodName", Times.Once(), logLevel);
        }

        [Theory]
        [InlineData(LogLevel.Critical, "LogCritical")]
        [InlineData(LogLevel.Debug, "LogDebug")]
        [InlineData(LogLevel.Error, "LogError")]
        [InlineData(LogLevel.Information, "LogInformation")]
        [InlineData(LogLevel.Trace, "LogTrace")]
        [InlineData(LogLevel.Warning, "LogWarning")]
        public void RouteCall_CallsInvokeLogMethod_WithTheCorrectName(LogLevel logLevel, string logMethodName)
        {
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;

            sut.RouteCall<object>(logLevel, new EventId(0), new object(), null);
            
            sutMock.Protected().Verify("InvokeLogMethod", Times.Once(), logMethodName, ItExpr.IsAny<object[]>());
        }

        [Theory]
        [InlineData(10, 3, typeof(Exception))]
        [InlineData(20, 5, typeof(InvalidOperationException))]
        [InlineData(30, 2, typeof(UnitTestForcedException))]
        public void RouteCall_CallsGetLogMethodParameters_WithTheExpectedParameters(int eventIdValue, int stateCount, Type exceptionType)
        {
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;

            var eventId = new EventId(eventIdValue);
            var state = new List<KeyValuePair<string, object>>();
            foreach (var idx in Enumerable.Range(1, stateCount))
            {
                state.Add(new($"key{idx}", Guid.NewGuid()));
            }
            var exception = Activator.CreateInstance(exceptionType) as Exception;

            sut.RouteCall(LogLevel.Information, eventId, state, exception);
            
            sutMock.Protected().Verify<object[]>("GetLogMethodParameters",
                new[]{ state.GetType() },
                Times.Once(),
                eventId, state, exception);
        }
        
        private static Mock<DefaultLogMethodRouter> GetTestableSubject()
        {
            var testableSut = GetTestableSubject(out _);
            return testableSut;
        }

        private static Mock<DefaultLogMethodRouter> GetTestableSubject(out Mock<TargetSpy> targetSpy)
        {
            targetSpy = new Mock<TargetSpy>();
            var testableSut = new Mock<DefaultLogMethodRouter>(targetSpy.Object) { CallBase = true };
            return testableSut;
        }
        
        private static object? InvokeProtected(IMock<DefaultLogMethodRouter> sutMock, string methodName, object?[] parameters)
        {
            var method = sutMock.Object.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            var result = method!.Invoke(sutMock.Object, parameters);
            return result;
        }
        
        private static object? InvokeProtected(IMock<DefaultLogMethodRouter> sutMock, string methodName, Type genericType, object?[] parameters)
        {
            var method = sutMock.Object.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            var generic = method!.MakeGenericMethod(genericType);
            var result = generic.Invoke(sutMock.Object, parameters);
            return result;
        }

        public abstract class TargetSpy
        {
            public abstract void SomeMethod(string someParameter);
        }
    }
}
