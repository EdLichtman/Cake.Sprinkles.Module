using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Tests.Models.ConfigurationValuesTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.ConfigurationValuesTests
{
    [TestFixture]
    internal class SprinklesConfigurationValuesTests : SprinklesTestBase
    {
        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(
               ConfigurationValuesTask.CakeEnvPropertyName,
               null,
               EnvironmentVariableTarget.Process);

            Environment.SetEnvironmentVariable(
               ConfigurationValuesTask.CakeConfigPropertyName,
               null,
               EnvironmentVariableTarget.Process);
        }

        [Test]
        public void SprinklesShouldGetConfigurationFromCakeConfigFiles()
        {
            GetCakeHost<ConfigurationValuesTask>()
                .UseWorkingDirectory(GetWorkingDirectory())
                .Run(FormatCustomArguments(nameof(ConfigurationValuesTask)));

            Assert.IsTrue(!string.IsNullOrWhiteSpace(GetContext<ConfigurationValuesTask>()?.CakeConfigProperty), "Cake Config value didn't populate properly.");
        }

        [Test]
        public void SprinklesShouldGetConfigurationFromEnvironmentVariables()
        {
            var expectedInput = "foobar";
            Environment.SetEnvironmentVariable(
                ConfigurationValuesTask.CakeEnvPropertyName, 
                expectedInput, 
                EnvironmentVariableTarget.Process);

            GetCakeHost<ConfigurationValuesTask>()
                .UseWorkingDirectory(GetWorkingDirectory())
                .Run(FormatCustomArguments(nameof(ConfigurationValuesTask)));

            Assert.That(GetContext<ConfigurationValuesTask>()?.CakeEnvProperty, Is.EqualTo(expectedInput), "Cake Env value didn't populate properly.");
        }

        [Test]
        public void SprinklesShouldPrioritizeArgumentsPassedInViaCommandLineAboveConfigurationFile()
        {
            var prioritizedValue = $"foobarthisismyuniqueproperty{Guid.NewGuid()}";
            GetCakeHost<ConfigurationValuesTask>()
                .UseWorkingDirectory(GetWorkingDirectory())
                .Run(
                    FormatCustomArguments(
                        nameof(ConfigurationValuesTask),
                        (ConfigurationValuesTask.CakeConfigPropertyName, prioritizedValue)));

            Assert.That(GetContext<ConfigurationValuesTask>()?.CakeConfigProperty, Is.EqualTo(prioritizedValue), "Cake Config value didn't prioritize properly.");
        }


        [Test]
        public void SprinklesShouldPrioritizeArgumentsPassedInViaCommandLineAboveEnvironmentVariables()
        {
            var prioritizedValue = $"foobarthisismyuniqueproperty{Guid.NewGuid()}";
            GetCakeHost<ConfigurationValuesTask>()
                .UseWorkingDirectory(GetWorkingDirectory())
                .Run(
                    FormatCustomArguments(
                        nameof(ConfigurationValuesTask),
                        (ConfigurationValuesTask.CakeEnvPropertyName, prioritizedValue)));

            Assert.That(GetContext<ConfigurationValuesTask>()?.CakeEnvProperty, Is.EqualTo(prioritizedValue), "Cake Env value didn't prioritize properly.");
        }


        [Test]
        public void SprinklesShouldPrioritizeArgumentsPassedInConfigurationFileAboveEnvironmentVariables()
        {
            var deprioritizedValue = $"foobarthisismyuniqueproperty{Guid.NewGuid()}";
            Environment.SetEnvironmentVariable(
                ConfigurationValuesTask.CakeConfigPropertyName,
                deprioritizedValue,
                EnvironmentVariableTarget.Process);

            GetCakeHost<ConfigurationValuesTask>()
                .UseWorkingDirectory(GetWorkingDirectory())
                .Run(
                    FormatCustomArguments(
                        nameof(ConfigurationValuesTask)));

            var context = GetContext<ConfigurationValuesTask>();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(context?.CakeConfigProperty));
            Assert.That(
                context?.CakeConfigProperty, 
                Is.Not.EqualTo(deprioritizedValue), 
                "Cake Env value didn't prioritize properly.");
        }

        private string GetWorkingDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var cakeConfigFile = new DirectoryInfo(currentDirectory).GetFiles("cake.config", SearchOption.AllDirectories).FirstOrDefault();

            Assert.That(cakeConfigFile, Is.Not.Null, "Could not find cake config file. this test needs to have Working directory contain cake config file.");
            return cakeConfigFile!.Directory!.FullName;
        }
    }
}
