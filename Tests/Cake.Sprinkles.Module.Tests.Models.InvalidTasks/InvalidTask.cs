using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using System.Collections.Immutable;

namespace Cake.Sprinkles.Module.Tests.Models.InvalidTasks
{
    [TaskName("Default")]
    public class InvalidTask : FrostingTask<SprinklesTestContext<InvalidTask>>
    {
        public const string MyRequiredPropertyDescription = "The description that will be shown to the user when a property is required that is not set.";

        [TaskArgumentName(nameof(MyInvalidEnumerableProperty))]
        public List<string> MyInvalidEnumerableProperty { get; set; }

        [TaskArgumentName(nameof(MyInvalidFlagProperty))]
        [TaskArgumentIsFlag]
        public string MyInvalidFlagProperty { get; set; }

        [TaskArgumentName(nameof(MyRequiredFlagProperty))]
        [TaskArgumentIsFlag]
        [TaskArgumentIsRequired]
        public bool MyRequiredFlagProperty { get; set; }

        [TaskArgumentName(nameof(MyEnumerableFlagProperty))]
        [TaskArgumentIsFlag]
        public ImmutableList<bool> MyEnumerableFlagProperty { get; set; }

        [TaskArgumentName(nameof(MyRequiredProperty))]
        [TaskArgumentDescription(MyRequiredPropertyDescription)]
        [TaskArgumentIsRequired]
        public string MyRequiredProperty { get; set; }

        [TaskArgumentName(nameof(MyDelimiterProperty))]
        [TaskArgumentEnumerationDelimiter(";")]
        public ImmutableList<string> MyDelimiterProperty { get; set; }

        [TaskArgumentName(nameof(MyInvalidDelimiterProperty))]
        [TaskArgumentEnumerationDelimiter(";")]
        public string MyInvalidDelimiterProperty { get; set; }
        public override void Run(SprinklesTestContext<InvalidTask> context)
        {
            context.Task = this;
        }
    }
}
