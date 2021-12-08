using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace su3dev.Logging.Moq.Tests
{
    public class LoggerMockTests
    {
        [Fact]
        public void Create_ReturnsNonNull()
        {
            var actual = LoggerMock.Create();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateGeneric_ReturnsNonNull()
        {
            var actual = LoggerMock.Create<LoggerCategory>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void Get_WithInvalidLogger_Throws()
        {
            var logger = Mock.Of<ILogger>();

            var exception = Assert.Throws<ArgumentException>(() => LoggerMock.Get(logger));
            exception.Message.Should()
                .StartWith(
                    $"Incompatible logger type. The logger must be compatible with type {typeof(Logger).FullName} in order for it to be properly mocked.");
        }

        [Fact]
        public void Get_WithValidLogger_ReturnsCorrectMock()
        {
            var logger = Mock.Of<Logger>();

            var actual = LoggerMock.Get(logger);

            var expected = Mock.Get(logger);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Get_WithInvalidLoggerGeneric_Throws()
        {
            var logger = Mock.Of<ILogger<LoggerCategory>>();

            var exception = Assert.Throws<ArgumentException>(() => LoggerMock.Get(logger));
            exception.Message.Should()
                .StartWith(
                    $"Incompatible logger type. The logger must be compatible with type {typeof(Logger<LoggerCategory>).FullName} in order for it to be properly mocked.");
        }

        [Fact]
        public void Get_WithValidLoggerGeneric_ReturnsCorrectMock()
        {
            var logger = Mock.Of<Logger<LoggerCategory>>();

            var actual = LoggerMock.Get(logger);

            var expected = Mock.Get(logger);
            actual.Should().Be(expected);
        }
        
        public class LoggerCategory
        { }
    }
}
