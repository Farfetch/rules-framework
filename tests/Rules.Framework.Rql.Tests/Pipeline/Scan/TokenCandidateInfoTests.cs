namespace Rules.Framework.Rql.Tests.Pipeline.Scan
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Xunit;

    public class TokenCandidateInfoTests
    {
        [Fact]
        public void MarkAsError_GivenAlreadyError_ThrowsInvalidOperationException()
        {
            // Arrange
            var tokenCandidateInfo = new TokenCandidateInfo(10, 2, 3);
            tokenCandidateInfo.MarkAsError("Existent error");

            // Act
            var actual = Assert.Throws<InvalidOperationException>(() => tokenCandidateInfo.MarkAsError("Test error"));

            // Assert
            actual.Should().NotBeNull();
            actual.Message.Should().Be("An error has already been reported for specified token candidate.");
        }

        [Fact]
        public void MarkAsError_GivenNoError_MarksAsErrorAndSetsMessage()
        {
            // Arrange
            var tokenCandidateInfo = new TokenCandidateInfo(10, 2, 3);

            // Act
            tokenCandidateInfo.MarkAsError("Test error");

            // Assert
            tokenCandidateInfo.HasError.Should().BeTrue();
            tokenCandidateInfo.Message.Should().Be("Test error");
        }

        [Fact]
        public void NextColumn_NoConditions_IncreasesEndOffsetAndColumnCount()
        {
            // Arrange
            var tokenCandidateInfo = new TokenCandidateInfo(10, 2, 3);

            // Act
            tokenCandidateInfo.NextColumn();

            // Assert
            tokenCandidateInfo.EndOffset.Should().Be(11);
            tokenCandidateInfo.EndPosition.Column.Should().Be(4);
            tokenCandidateInfo.EndPosition.Line.Should().Be(2);
            tokenCandidateInfo.Length.Should().Be(2);
        }

        [Fact]
        public void NextLine_NoConditions_IncreasesEndOffsetAndLineCount()
        {
            // Arrange
            var tokenCandidateInfo = new TokenCandidateInfo(10, 2, 3);

            // Act
            tokenCandidateInfo.NextLine();

            // Assert
            tokenCandidateInfo.EndOffset.Should().Be(11);
            tokenCandidateInfo.EndPosition.Column.Should().Be(1);
            tokenCandidateInfo.EndPosition.Line.Should().Be(3);
            tokenCandidateInfo.Length.Should().Be(2);
        }
    }
}