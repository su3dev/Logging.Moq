using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace su3dev.Logging.Moq.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void DefaultCtor_SetsRouter_WithDefaultRouter()
        {
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;

            var actual = sut.LogMethodRouter;

            actual.Should().NotBeNull();
            actual.Should().BeOfType<DefaultLogMethodRouter>();
        }

        [Fact]
        public void SettingLogMethodRouter_OverwritesLogMethodRouter()
        {
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;
            var logMethodRouter = Mock.Of<ILogMethodRouter>();
            
            sut.LogMethodRouter = logMethodRouter;
            
            var actual = sut.LogMethodRouter;

            actual.Should().Be(logMethodRouter);
        }

        [Theory]
        [InlineData(LogLevel.Information, 10, 3, typeof(Exception))]
        [InlineData(LogLevel.Warning, 20, 5, typeof(InvalidOperationException))]
        [InlineData(LogLevel.Critical, 30, 2, typeof(UnitTestForcedException))]
        public void Log_CallsRouteCall_OnLogMethodRouter(LogLevel logLevel, int eventIdValue, int stateCount, Type exceptionType)
        {
            var eventId = new EventId(eventIdValue);
            var state = new List<KeyValuePair<string, object>>();
            foreach (var idx in Enumerable.Range(1, stateCount))
            {
                state.Add(new($"key{idx}", Guid.NewGuid()));
            }
            var exception = Activator.CreateInstance(exceptionType) as Exception;
            
            var sutMock = GetTestableSubject();
            var sut = sutMock.Object;
            var logMethodRouterMock = new Mock<ILogMethodRouter>();
            sut.LogMethodRouter = logMethodRouterMock.Object;
            
            sut.Log(logLevel, eventId, state, exception, (_, _) => string.Empty);

            logMethodRouterMock.Verify(m => m.RouteCall(logLevel, eventId, state, exception));
        }
        
        private static Mock<Logger> GetTestableSubject()
        {
            var testableSut = new Mock<Logger>() { CallBase = true };
            return testableSut;
        }
    }
}
