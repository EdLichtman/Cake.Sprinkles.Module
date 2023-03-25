using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;

namespace Cake.Sprinkles.Module.Tests.Models.ConfigurationValuesTasks
{
    [TaskName(nameof(ConfigurationValuesTask))]
    public class ConfigurationValuesTask : FrostingTask<SprinklesTestContext<ConfigurationValuesTask>>
    {
        public const string CakeConfigPropertyName = "custom_cake_sprinkles_test_cake_config_property";
        public const string CakeEnvPropertyName = "custom_cake_sprinkles_test_cake_env_property";
        public const string CakeCliPropertyName = "custom_cake_sprinkles_test_cake_cli_property";

        [TaskArgumentName(CakeConfigPropertyName)]
        public string? CakeConfigProperty { get; set; }

        [TaskArgumentName(CakeEnvPropertyName)]
        public string? CakeEnvProperty { get; set; }

        [TaskArgumentName(CakeCliPropertyName)]
        public string? CakeCliProperty { get; set; } 

        public override void Run(SprinklesTestContext<ConfigurationValuesTask> context)
        {
            context.Task = this;
        }
    }
}
