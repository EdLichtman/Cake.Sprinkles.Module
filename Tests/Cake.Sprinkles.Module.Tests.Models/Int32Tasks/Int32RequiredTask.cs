using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.Tests.Models.Int32Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Int32Tasks
{
    [TaskName(nameof(Int32RequiredTask))]
    public class Int32RequiredTask : FrostingTask<SprinklesTestContext<Int32RequiredTask>>
    {
        [TaskArgumentName(nameof(Single))]
        [TaskArgumentIsRequired]
        public int Single { get; set; }

        [TaskArgumentName(nameof(List))]
        [TaskArgumentIsRequired]
        public ImmutableList<int> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<int> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<Int32RequiredTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
