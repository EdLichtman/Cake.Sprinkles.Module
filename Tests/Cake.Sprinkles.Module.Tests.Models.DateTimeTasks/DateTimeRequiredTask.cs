using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.DateTimeTasks
{
    [TaskName(nameof(DateTimeRequiredTask))]
    public class DateTimeRequiredTask : FrostingTask<SprinklesTestContext<DateTimeRequiredTask>>
    {
        [TaskArgumentName(nameof(Single))]
        [TaskArgumentIsRequired]
        public DateTime Single { get; set; }

        [TaskArgumentName(nameof(List))]
        [TaskArgumentIsRequired]
        public ImmutableList<DateTime> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<DateTime> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<DateTimeRequiredTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
