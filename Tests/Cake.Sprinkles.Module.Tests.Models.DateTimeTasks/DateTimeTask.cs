using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.DateTimeTasks
{
    [TaskName("Default")]
    public class DateTimeTask : FrostingTask<SprinklesTestContext<DateTimeTask>>
    {
        [TaskArgumentName("required_single")]
        [TaskArgumentIsRequired]
        public DateTime RequiredSingle { get; set; }

        [TaskArgumentName("required_list")]
        [TaskArgumentIsRequired]
        public ImmutableList<DateTime> RequiredList { get; set; } = null!;

        [TaskArgumentName("required_hashset")]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<DateTime> RequiredHashSet { get; set; } = null!;

        [TaskArgumentName("optional_single")]
        public DateTime? OptionalSingle { get; set; }


        [TaskArgumentName("optional_list")] 
        public ImmutableList<DateTime> OptionalList { get; set; } = null!;

        [TaskArgumentName("optional_hashset")]
        public ImmutableHashSet<DateTime> OptionalHashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<DateTimeTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
