namespace Rules.Framework.Tests
{
    using FluentAssertions;
    using Xunit;

    public class RuleAddPriorityOptionTests
    {
        [Fact]
        public void AtBottom_NoConditions_ReturnsNewObjectSettedAtBottomPriority()
        {
            // Arrange
            PriorityOptions priorityOption = PriorityOptions.AtBottom;

            // Act
            RuleAddPriorityOption actual = RuleAddPriorityOption.AtBottom;

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void AtTop_NoConditions_ReturnsNewObjectSettedAtTopPriority()
        {
            // Arrange
            PriorityOptions priorityOption = PriorityOptions.AtTop;

            // Act
            RuleAddPriorityOption actual = RuleAddPriorityOption.AtTop;

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void ByPriorityNumber_GivenPriority1_ReturnsNewObjectSettedByPriorityNumberAndAtPriority1()
        {
            // Arrange
            int priorityNumber = 1;
            PriorityOptions priorityOption = PriorityOptions.AtPriorityNumber;

            // Act
            RuleAddPriorityOption actual = RuleAddPriorityOption.ByPriorityNumber(priorityNumber);

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(priorityNumber);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void ByRuleName_GivenRuleNameSample_ReturnsNewObjectSettedByRuleNamePriorityAndAtNameSample()
        {
            // Arrange
            string ruleName = "Sample";
            PriorityOptions priorityOption = PriorityOptions.AtRuleName;

            // Act
            RuleAddPriorityOption actual = RuleAddPriorityOption.ByRuleName(ruleName);

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().Be(ruleName);
            actual.PriorityOption.Should().Be(priorityOption);
        }
    }
}