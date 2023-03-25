using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.BooleanTasks
{
    [TaskName(nameof(BooleanOptionalTask))]
    public class BooleanOptionalTask : FrostingTask<SprinklesTestContext<BooleanOptionalTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public bool Single { get; set; }

        [TaskArgumentName(nameof(List))]
        public ImmutableList<bool> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        public ImmutableHashSet<bool> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<BooleanOptionalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
