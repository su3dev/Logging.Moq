using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace su3dev.Logging.Moq.Tests
{
    public class LoggerInterceptorTests
    {
        private Mock<ILogger> LoggerMock { get; }
        private ILogger Logger => LoggerMock.Object;

        public LoggerInterceptorTests()
        {
            LoggerMock = new Mock<ILogger>();
        }
        
        [Fact]
        public void LogInformation_CapturesLevelAndText()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            Logger.LogInformation(expectedText);
            
            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Information, Text = expectedText, Exception = (object)null! }
            });
        }

        [Fact]
        public void LogWarning_CapturesLevelAndText()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            Logger.LogWarning(expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Warning, Text = expectedText, Exception = (object)null! }
            });
        }

        [Fact]
        public void LogWarning_CapturesExceptionIfProvided()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            var expectedException = new Fixture().Create<Exception>();
            Logger.LogWarning(expectedException, expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Warning, Text = expectedText, Exception = expectedException }
            });
        }

        [Fact]
        public void LogError_CapturesLevelAndText()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            Logger.LogError(expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Error, Text = expectedText, Exception = (object)null! }
            });
        }

        [Fact]
        public void LogError_CapturesExceptionIfProvided()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            var expectedException = new Fixture().Create<Exception>();
            Logger.LogError(expectedException, expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Error, Text = expectedText, Exception = expectedException }
            });
        }

        [Fact]
        public void LogCritical_CapturesLevelAndText()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            Logger.LogCritical(expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Critical, Text = expectedText, Exception = (object)null! }
            });
        }

        [Fact]
        public void LogCritical_CapturesExceptionIfProvided()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            var expectedException = new Fixture().Create<Exception>();
            Logger.LogCritical(expectedException, expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Critical, Text = expectedText, Exception = expectedException }
            });
        }

        [Fact]
        public void LogDebug_CapturesLevelAndText()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            Logger.LogDebug(expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Debug, Text = expectedText, Exception = (object)null! }
            });
        }

        [Fact]
        public void LogDebug_CapturesExceptionIfProvided()
        {
            var sut = LoggerInterceptor.Create(LoggerMock);

            const string expectedText = "some-text";
            var expectedException = new Fixture().Create<Exception>();
            Logger.LogDebug(expectedException, expectedText);

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Debug, Text = expectedText, Exception = expectedException }
            });
        }

        [Fact]
        public void LogsMultipleMessages()
        {
            var loggerMock = new Mock<ILogger>();
            var logger = loggerMock.Object;

            var sut = LoggerInterceptor.Create(loggerMock);

            logger.LogInformation("message-1");
            logger.LogInformation("message-2");
            logger.LogInformation("message-3");

            sut.LoggedMessages.Should().BeEquivalentTo(new[]
            {
                new { Level = LogLevel.Information, Text = "message-1" },
                new { Level = LogLevel.Information, Text = "message-2" },
                new { Level = LogLevel.Information, Text = "message-3" },
            });
        }
    }
}
