using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System.Collections.Immutable;

namespace Cake.Sprinkles.Module.Tests.Models.StringTasks
{
    [TaskName(nameof(StringOptionalTask))]
    public class StringOptionalTask : FrostingTask<SprinklesTestContext<StringOptionalTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public string Single { get; set; } = null!;

        [TaskArgumentName(nameof(List))]
        public ImmutableList<string> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        public ImmutableHashSet<string> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<StringOptionalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
