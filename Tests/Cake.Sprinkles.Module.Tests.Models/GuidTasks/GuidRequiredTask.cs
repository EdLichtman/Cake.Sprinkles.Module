using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;

namespace Cake.Sprinkles.Module.Tests.Models.GuidTasks
{
    [TaskName(nameof(GuidRequiredTask))]
    public class GuidRequiredTask : FrostingTask<SprinklesTestContext<GuidRequiredTask>>
    {
        [TaskArgumentName(nameof(Single))]
        [TaskArgumentIsRequired]
        public Guid Single { get; set; }

        [TaskArgumentName(nameof(List))]
        [TaskArgumentIsRequired]
        public ImmutableList<Guid> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<Guid> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<GuidRequiredTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
