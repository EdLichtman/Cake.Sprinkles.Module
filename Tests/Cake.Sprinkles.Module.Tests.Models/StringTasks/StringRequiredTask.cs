using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;

namespace Cake.Sprinkles.Module.Tests.Models.StringTasks
{
    [TaskName(nameof(StringRequiredTask))]
    public class StringRequiredTask : FrostingTask<SprinklesTestContext<StringRequiredTask>>
    {
        [TaskArgumentName(nameof(Single))]
        [TaskArgumentIsRequired]
        public string Single { get; set; } = null!;

        [TaskArgumentName(nameof(List))]
        [TaskArgumentIsRequired]
        public ImmutableList<string> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<string> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<StringRequiredTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
