namespace Rules.Framework.IntegrationTests.Scenarios.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario3;
    using Xunit;

    public class BuildingSecuritySystemControlTests
    {
        private static string DataSourceFilePath => $@"{Environment.CurrentDirectory}/Scenarios/Scenario3/rules-framework-tests.security-system-actionables.json";

        [Fact]
        public async Task BuildingSecuritySystem_FireScenario_ReturnsActionsToTrigger()
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.FireSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<SecuritySystemConditions, object>
            {
                { SecuritySystemConditions.TemperatureCelsius, 100.0m },
                { SecuritySystemConditions.SmokeRate, 55 },
                { SecuritySystemConditions.PowerStatus, "Online" },
            };

            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(genericRulesEngine, DataSourceFilePath, typeof(SecuritySystemAction));

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList()
                .Should().Contain(ssa => ssa.ActionName == "CallFireBrigade")
                .And.Contain(ssa => ssa.ActionName == "CallPolice")
                .And.Contain(ssa => ssa.ActionName == "ActivateSprinklers")
                .And.HaveCount(3);
        }

        [Fact]
        public async Task BuildingSecuritySystem_PowerFailureScenario_ReturnsActionsToTrigger()
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<SecuritySystemConditions, object>
            {
                { SecuritySystemConditions.TemperatureCelsius, 100.0m },
                { SecuritySystemConditions.SmokeRate, 55 },
                { SecuritySystemConditions.PowerStatus, "Offline" },
            };

            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(genericRulesEngine, DataSourceFilePath, typeof(SecuritySystemAction));

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList()
                .Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.Contain(ssa => ssa.ActionName == "EnableEmergencyPower")
                .And.Contain(ssa => ssa.ActionName == "CallPowerGridPicket");
        }

        [Fact]
        public async Task BuildingSecuritySystem_PowerShutdownScenario_ReturnsActionsToTrigger()
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<SecuritySystemConditions, object>
            {
                { SecuritySystemConditions.TemperatureCelsius,100.0m },
                { SecuritySystemConditions.SmokeRate,55 },
                { SecuritySystemConditions.PowerStatus,"Shutdown" },
            };

            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(genericRulesEngine, DataSourceFilePath, typeof(SecuritySystemAction));

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList()
                .Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.HaveCount(1);
        }
    }
}