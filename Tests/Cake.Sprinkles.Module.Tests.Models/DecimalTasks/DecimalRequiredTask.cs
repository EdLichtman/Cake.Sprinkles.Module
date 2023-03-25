using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;

namespace Cake.Sprinkles.Module.Tests.Models.DecimalTasks
{
    [TaskName(nameof(DecimalRequiredTask))]
    public class DecimalRequiredTask : FrostingTask<SprinklesTestContext<DecimalRequiredTask>>
    {
        [TaskArgumentName(nameof(Single))]
        [TaskArgumentIsRequired]
        public decimal Single { get; set; }

        [TaskArgumentName(nameof(List))]
        [TaskArgumentIsRequired]
        public ImmutableList<decimal> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<decimal> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<DecimalRequiredTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
