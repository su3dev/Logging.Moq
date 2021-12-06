using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace su3dev.Logging.Moq.Tests
{
    public class DefaultLogMethodRouterTests
    {
        [Fact]
        public void Ctor_SetsTarget()
        {
            var targetMock = new Mock<TargetSpy>();
            var sut = new DefaultLogMethodRouter(targetMock.Object);

            sut.Target.Should().Be(targetMock.Object);
        }

        [Fact]
        public void InvokeLogMethod_CallsTarget()
        {
            var targetMock = new Mock<TargetSpy>();
            var sutMock = new Mock<DefaultLogMethodRouter>(targetMock.Object) { CallBase = true };

            _ = InvokeProtected(sutMock, "InvokeLogMethod",
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
            var targetMock = new Mock<TargetSpy>();
            var sutMock = new Mock<DefaultLogMethodRouter>(targetMock.Object) { CallBase = true };

            var actual = InvokeProtected(sutMock, "GetLogMethodName", new object[] { logLevel });

            actual.Should().Be(expectedMethodName);
        }

        [Theory]
        //          event id, should contain it
        [InlineData(       1,              true)]
        [InlineData(       0,             false)]
        public void GetLogMethodParameters_AddsEventId_OnlyWhenEventIdIsNotDefault(int eventId, bool shouldContainEventId)
        {
            var targetMock = new Mock<TargetSpy>();
            var sutMock = new Mock<DefaultLogMethodRouter>(targetMock.Object) { CallBase = true };

            var eventIdInstance = new EventId(eventId);
            var actual = InvokeProtected(sutMock, "GetLogMethodParameters", typeof(object), new[] { eventIdInstance, new object(), null });

            actual.Should().BeAssignableTo<object[]>();
            var actualArray = actual as object[];
            if (shouldContainEventId)
            {
                actualArray.Should().Contain(eventIdInstance);
            }
            else
            {
                actualArray.Should().NotContain(eventIdInstance);
            }
        }

        [Theory]
        //          add exception, should contain it
        [InlineData(         true,              true)]
        [InlineData(        false,             false)]
        public void GetLogMethodParameters_AddsException_OnlyWhenExceptionIsNotNull(bool addException, bool shouldContainException)
        {
            var targetMock = new Mock<TargetSpy>();
            var sutMock = new Mock<DefaultLogMethodRouter>(targetMock.Object) { CallBase = true };

            var exception = new UnitTestForcedException();
            var actual = InvokeProtected(sutMock, "GetLogMethodParameters", typeof(object),
                new[] { new EventId(0), new object(), addException ? exception : null });

            actual.Should().BeAssignableTo<object[]>();
            var actualArray = actual as object[];
            if (shouldContainException)
            {
                actualArray.Should().Contain(exception!);
            }
            else
            {
                actualArray.Should().NotContain(exception!);
            }
        }

        [Fact]
        public void GetLogMethodParameters_AddsStringWithNullText_WhenStateIsNotOfTheCorrectType()
        {
            var targetMock = new Mock<TargetSpy>();
            var sutMock = new Mock<DefaultLogMethodRouter>(targetMock.Object) { CallBase = true };

            var actual = InvokeProtected(sutMock, "GetLogMethodParameters", typeof(object),
                new[] { new EventId(0), new object(), null });

            actual.Should().BeAssignableTo<object[]>();
            var actualArray = actual as object[];
            actualArray.Should().Contain("[null]");
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
