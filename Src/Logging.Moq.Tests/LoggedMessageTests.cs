using AutoFixture;
using FluentAssertions;
using Xunit;

namespace su3dev.Logging.Moq.Tests
{
    public class LoggedMessageTests
    {
        [Fact]
        public void AllProperties()
        {
            var expected = new Fixture().Create<LoggedMessage>();

            var sut = new LoggedMessage(expected.Level, expected.EventId, expected.Text, expected.Exception);

            sut.Level.Should().Be(expected.Level);
            sut.EventId.Should().Be(expected.EventId);
            sut.Text.Should().Be(expected.Text);
            sut.Exception.Should().Be(expected.Exception);
        }
    }
}
