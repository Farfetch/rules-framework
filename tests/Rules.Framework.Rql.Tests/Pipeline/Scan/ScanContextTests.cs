namespace Rules.Framework.Rql.Tests.Pipeline.Scan
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Xunit;

    public class ScanContextTests
    {
        private readonly string source;

        public ScanContextTests()
        {
            this.source = "MATCH ONE RULE FOR \"Test\" ON $2023-01-01Z$;\nMATCH ONE RULE FOR \"Other\nTest\" ON $2024-01-01Z$;";
        }

        [Fact]
        public void BeginTokenCandidate_AlreadyHasTokenCandidateCreated_ThrowsInvalidOperationException()
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            _ = scanContext.BeginTokenCandidate();

            // Act
            var actual = Assert.Throws<InvalidOperationException>(() => scanContext.BeginTokenCandidate());

            // Assert
            actual.Should().NotBeNull();
            actual.Message.Should().Be("A token candidate is currently created. Cannot begin a new one.");
        }

        [Fact]
        public void BeginTokenCandidate_NoTokenCandidateCreated_ReturnsDisposableScope()
        {
            // Arrange
            var scanContext = new ScanContext(this.source);

            // Act
            var tokenCandidateScope = scanContext.BeginTokenCandidate();

            // Assert
            tokenCandidateScope.Should().NotBeNull()
                .And.BeAssignableTo<IDisposable>();
            scanContext.TokenCandidate.Should().NotBeNull();
            scanContext.TokenCandidate.BeginPosition.Column.Should().Be(0);
            scanContext.TokenCandidate.BeginPosition.Line.Should().Be(1);
            scanContext.TokenCandidate.StartOffset.Should().Be(0);
            scanContext.TokenCandidate.EndOffset.Should().Be(0);
            scanContext.TokenCandidate.EndPosition.Column.Should().Be(0);
            scanContext.TokenCandidate.EndPosition.Line.Should().Be(1);
        }

        [Fact]
        public void ExtractLexeme_NoTokenCandidate_ThrowsInvalidOperationException()
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < 4; i++)
            {
                _ = scanContext.MoveNext();
            }

            // Act
            var actual = Assert.Throws<InvalidOperationException>(() => scanContext.ExtractLexeme());

            // Assert
            actual.Should().BeOfType<InvalidOperationException>();
            actual.Message.Should().Be("Must be on a token candidate scope. Ensure you have invoked BeginTokenCandidate() " +
                "and extract lexeme before disposing of token candidate.");
        }

        [Theory]
        [InlineData(0, 4, "MATCH")] // Token without newline
        [InlineData(63, 11, "\"Other\nTest\"")] // Token with newline
        public void ExtractLexeme_TokenCandidateCreated_ReturnsTokenStringRepresentation(int numberOfMoves, int numberOfChars, string expected)
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < numberOfMoves; i++)
            {
                _ = scanContext.MoveNext();
            }

            using (scanContext.BeginTokenCandidate())
            {
                for (int i = 0; i < numberOfChars; i++)
                {
                    _ = scanContext.MoveNext();
                }

                // Act
                var actual = scanContext.ExtractLexeme();

                // Assert
                actual.Should().Be(expected);
            }
        }

        [Theory]
        [InlineData(0, 'M')]
        [InlineData(2, 'T')]
        [InlineData(10, 'R')]
        [InlineData(93, ';')]
        public void GetCurrentChar_Conditions_ReturnsChar(int numberOfMoves, char expected)
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < numberOfMoves; i++)
            {
                _ = scanContext.MoveNext();
            }

            // Act
            var actual = scanContext.GetCurrentChar();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(0, 'A')]
        [InlineData(2, 'C')]
        [InlineData(10, 'U')]
        [InlineData(93, '\0')]
        public void GetNextChar_Conditions_ReturnsChar(int numberOfMoves, char expected)
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < numberOfMoves; i++)
            {
                _ = scanContext.MoveNext();
            }

            // Act
            var actual = scanContext.GetNextChar();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(2, false)]
        [InlineData(93, true)]
        public void IsEof_Conditions_ReturnsBool(int numberOfMoves, bool expected)
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < numberOfMoves; i++)
            {
                _ = scanContext.MoveNext();
            }

            // Act
            var actual = scanContext.IsEof();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(0, true, 1)]
        [InlineData(2, true, 3)]
        [InlineData(92, false, 92)]
        public void MoveNext_Conditions_ReturnsBool(int numberOfMoves, bool expected, int expectedOffset)
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < numberOfMoves; i++)
            {
                _ = scanContext.MoveNext();
            }

            // Act
            var actual = scanContext.MoveNext();

            // Assert
            actual.Should().Be(expected);
            scanContext.Offset.Should().Be(expectedOffset);
        }

        [Theory]
        [InlineData(0, 'A', true, 1)]
        [InlineData(0, 'T', false, 0)]
        [InlineData(2, 'C', true, 3)]
        [InlineData(2, 'H', false, 2)]
        [InlineData(92, '\0', false, 92)]
        public void MoveNextConditionally_Conditions_ReturnsBool(int numberOfMoves, char nextChar, bool expected, int expectedOffset)
        {
            // Arrange
            var scanContext = new ScanContext(this.source);
            for (int i = 0; i < numberOfMoves; i++)
            {
                _ = scanContext.MoveNext();
            }

            // Act
            var actual = scanContext.MoveNextConditionally(nextChar);

            // Assert
            actual.Should().Be(expected);
            scanContext.Offset.Should().Be(expectedOffset);
        }
    }
}