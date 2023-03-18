using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.DecimalTasks
{
    [TaskName("Default")]
    public class DecimalTask : FrostingTask<SprinklesTestContext<DecimalTask>>
    {
        [TaskArgumentName("required_single")]
        [TaskArgumentIsRequired]
        public Decimal RequiredSingle { get; set; }

        [TaskArgumentName("required_list")]
        [TaskArgumentIsRequired]
        public ImmutableList<Decimal> RequiredList { get; set; } = null!;

        [TaskArgumentName("required_hashset")]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<Decimal> RequiredHashSet { get; set; } = null!;

        [TaskArgumentName("optional_single")]
        public Decimal? OptionalSingle { get; set; }


        [TaskArgumentName("optional_list")] 
        public ImmutableList<Decimal> OptionalList { get; set; } = null!;

        [TaskArgumentName("optional_hashset")]
        public ImmutableHashSet<Decimal> OptionalHashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<DecimalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
