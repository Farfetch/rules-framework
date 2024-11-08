namespace Rules.Framework.WebUI.Tests.Services
{
    using System;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Rules.Framework.WebUI.Services;
    using Xunit;

    public class RulesEngineInstanceProviderTests
    {
        [Fact]
        public void RegisterOneInstanceTest_Success()
        {
            // Arrange
            var name = "Sample engine";
            var instanceId = Guid.Parse("aeccf6a8-7851-cda0-4e2b-802a97707225");
            var rulesEngine = Mock.Of<IRulesEngine>();

            var serviceProvider = Mock.Of<IServiceProvider>();
            Mock.Get(serviceProvider)
                .Setup(x => x.GetService(typeof(IRulesEngine)))
                .Returns(rulesEngine);
            var rulesEngineInstanceProvider = new RulesEngineInstanceProvider();

            // Act
            rulesEngineInstanceProvider.AddInstance(name, (sp, _) => sp.GetService<IRulesEngine>());
            rulesEngineInstanceProvider.EnumerateInstances(serviceProvider);
            var instance = rulesEngineInstanceProvider.GetInstance(instanceId);
            var instances = rulesEngineInstanceProvider.GetAllInstances();

            // Assert
            instance.Should().NotBeNull();
            instance.Id.Should().Be(instanceId);
            instance.Name.Should().Be(name);
            instance.RulesEngine.Should().BeSameAs(rulesEngine);
            instances.Should().NotBeNull()
                .And.HaveCount(1)
                .And.Contain(instance);
        }

        [Fact]
        public void RegisterTwoInstanceSameNameTest_Failure()
        {
            // Arrange
            var name = "Sample engine";

            var rulesEngineInstanceProvider = new RulesEngineInstanceProvider();

            // Act
            rulesEngineInstanceProvider.AddInstance(name, (sp, _) => sp.GetService<IRulesEngine>());
            var exception = Assert.Throws<InvalidOperationException>(
                () => rulesEngineInstanceProvider.AddInstance(name, (sp, _) => sp.GetService<IRulesEngine>()));

            // Assert
            exception.Message.Should().Contain(name);
        }

        [Fact]
        public void RegisterTwoInstanceTest_Success()
        {
            // Arrange
            var name1 = "Sample engine";
            var instanceId1 = Guid.Parse("aeccf6a8-7851-cda0-4e2b-802a97707225");
            var rulesEngine1 = Mock.Of<IRulesEngine>();
            var name2 = "Another sample engine";
            var instanceId2 = Guid.Parse("1c45bfc8-7dfb-f399-adbf-0976e00d3e3e");
            var rulesEngine2 = Mock.Of<IRulesEngine>();

            var serviceProvider = Mock.Of<IServiceProvider>();
            Mock.Get(serviceProvider)
                .SetupSequence(x => x.GetService(typeof(IRulesEngine)))
                .Returns(rulesEngine1)
                .Returns(rulesEngine2);
            var rulesEngineInstanceProvider = new RulesEngineInstanceProvider();

            // Act
            rulesEngineInstanceProvider.AddInstance(name1, (sp, _) => sp.GetService<IRulesEngine>());
            rulesEngineInstanceProvider.AddInstance(name2, (sp, _) => sp.GetService<IRulesEngine>());
            rulesEngineInstanceProvider.EnumerateInstances(serviceProvider);
            var instance1 = rulesEngineInstanceProvider.GetInstance(instanceId1);
            var instance2 = rulesEngineInstanceProvider.GetInstance(instanceId2);
            var instances = rulesEngineInstanceProvider.GetAllInstances();

            // Assert
            instance1.Should().NotBeNull();
            instance1.Id.Should().Be(instanceId1);
            instance1.Name.Should().Be(name1);
            instance1.RulesEngine.Should().BeSameAs(rulesEngine1);
            instance2.Should().NotBeNull();
            instance2.Id.Should().Be(instanceId2);
            instance2.Name.Should().Be(name2);
            instance2.RulesEngine.Should().BeSameAs(rulesEngine2);
            instances.Should().NotBeNull()
                .And.HaveCount(2)
                .And.Contain(instance1)
                .And.Contain(instance2);
        }
    }
}