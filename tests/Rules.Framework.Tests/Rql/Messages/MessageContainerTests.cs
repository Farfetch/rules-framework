namespace Rules.Framework.Tests.Rql.Messages
{
    using FluentAssertions;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Messages;
    using Xunit;

    public class MessageContainerTests
    {
        [Fact]
        public void Ctor_NoParameters_NoMessagesAndZeroCounts()
        {
            // Act
            var messageContainer = new MessageContainer();

            // Assert
            messageContainer.Messages.Should().BeEmpty();
            messageContainer.ErrorsCount.Should().Be(0);
            messageContainer.WarningsCount.Should().Be(0);
        }

        [Fact]
        public void Error_GivenParameters_AddsMessageAndIncreasesErrorCount()
        {
            // Arrange
            var text = "This is a message";
            var beginPosition = RqlSourcePosition.From(1, 1);
            var endPosition = RqlSourcePosition.From(1, 10);

            var messageContainer = new MessageContainer();

            // Act
            messageContainer.Error(text, beginPosition, endPosition);

            // Assert
            messageContainer.Messages.Should().HaveCount(1)
                .And.ContainSingle(m => m.Text == text
                    && object.Equals(m.EndPosition, endPosition)
                    && object.Equals(m.BeginPosition, beginPosition)
                    && m.Severity == MessageSeverity.Error);
            messageContainer.ErrorsCount.Should().Be(1);
            messageContainer.WarningsCount.Should().Be(0);
        }

        [Fact]
        public void Warning_GivenParameters_AddsMessageAndIncreasesWarningCount()
        {
            // Arrange
            var text = "This is a message";
            var beginPosition = RqlSourcePosition.From(1, 1);
            var endPosition = RqlSourcePosition.From(1, 10);

            var messageContainer = new MessageContainer();

            // Act
            messageContainer.Warning(text, beginPosition, endPosition);

            // Assert
            messageContainer.Messages.Should().HaveCount(1)
                .And.ContainSingle(m => m.Text == text
                    && object.Equals(m.EndPosition, endPosition)
                    && object.Equals(m.BeginPosition, beginPosition)
                    && m.Severity == MessageSeverity.Warning);
            messageContainer.ErrorsCount.Should().Be(0);
            messageContainer.WarningsCount.Should().Be(1);
        }
    }
}