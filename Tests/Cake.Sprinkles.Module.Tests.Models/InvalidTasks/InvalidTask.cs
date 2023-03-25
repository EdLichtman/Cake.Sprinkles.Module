using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.Tests.Models.Validation;
using System.Collections.Immutable;

namespace Cake.Sprinkles.Module.Tests.Models.InvalidTasks
{
    [TaskName(nameof(InvalidTask))]
    public class InvalidTask : FrostingTask<SprinklesTestContext<InvalidTask>>
    {
        public const string MyDuplicateArgumentName = nameof(MyDuplicateArgumentName);
        public const string MyRequiredPropertyDescription = "The description that will be shown to the user when a property is required that is not set.";

        [TaskArgumentName(nameof(MyInvalidEnumerableProperty))]
        public List<string> MyInvalidEnumerableProperty { get; set; } = null!;

        [TaskArgumentName(nameof(MyInvalidFlagProperty))]
        [TaskArgumentIsFlag]
        public string MyInvalidFlagProperty { get; set; } = null!;

        [TaskArgumentName(nameof(MyRequiredFlagProperty))]
        [TaskArgumentIsFlag]
        [TaskArgumentIsRequired]
        public bool MyRequiredFlagProperty { get; set; }

        [TaskArgumentName(nameof(MyEnumerableFlagProperty))]
        [TaskArgumentIsFlag]
        public ImmutableList<bool> MyEnumerableFlagProperty { get; set; } = null!;

        [TaskArgumentName(nameof(MyValidationFlagProperty))]
        [TaskArgumentIsFlag]
        [ValidateIntegerGreaterThanOne]
        public bool MyValidationFlagProperty { get; set; }

        [TaskArgumentName(nameof(MyRequiredProperty))]
        [TaskArgumentDescription(MyRequiredPropertyDescription)]
        [TaskArgumentIsRequired]
        public string MyRequiredProperty { get; set; } = null!;

        [TaskArgumentName(nameof(MyDelimiterProperty))]
        [TaskArgumentEnumerationDelimiter(";")]
        public ImmutableList<string> MyDelimiterProperty { get; set; } = null!;

        [TaskArgumentName(nameof(MyInvalidDelimiterProperty))]
        [TaskArgumentEnumerationDelimiter(";")]
        public string MyInvalidDelimiterProperty { get; set; } = null!;

        [TaskArgumentName(MyDuplicateArgumentName)]
        public string ParentArgument { get; set; } = null!;

        [TaskArguments]
        public ChildInvalidArguments ChildInvalidTask { get; set; } = null!;

        [TaskArguments]
        [TaskArgumentName(nameof(ParentInvalidChildArguments))]
        public ParentInvalidChildArguments ParentInvalidChildArguments { get;set; } = null!;

        [TaskArguments]
        public ParameterConstructorClassChildArguments ParameterConstructorClassChildArguments { get; set; } = null!;

        [TaskArguments]
        public ParameterConstructorStructChildArguments ParameterConstructorStructChildArguments { get; set; }
        public override void Run(SprinklesTestContext<InvalidTask> context)
        {
            context.Task = this;
        }
    }
}
