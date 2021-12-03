using AutoFixture;
using FluentAssertions;
using Xunit;
// ReSharper disable CheckNamespace

namespace Moq.Tests
{
    public class LoggedMessageFixture
    {
        [Fact]
        public void AllProperties()
        {
            var expected = new Fixture().Create<LoggedMessage>();

            var sut = new LoggedMessage(expected.Level, expected.EventId, expected.Text, expected.Exception);

            sut.Level.Should().Be(expected.Level);
            // ReSharper disable HeapView.BoxingAllocation
            sut.EventId.Should().Be(expected.EventId);
            // ReSharper restore HeapView.BoxingAllocation
            sut.Text.Should().Be(expected.Text);
            sut.Exception.Should().Be(expected.Exception);
        }
    }
}
