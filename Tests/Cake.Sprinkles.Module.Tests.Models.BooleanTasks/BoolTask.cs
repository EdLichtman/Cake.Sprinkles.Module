using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.BooleanTasks
{
    [TaskName("Default")]
    public class BoolTask : FrostingTask<SprinklesTestContext<BoolTask>>
    {
        [TaskArgumentName("required_single")]
        [TaskArgumentIsRequired]
        public bool RequiredSingle { get; set; }

        [TaskArgumentName("required_list")]
        [TaskArgumentIsRequired]
        public ImmutableList<bool> RequiredList { get; set; } = null!;

        [TaskArgumentName("required_hashset")]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<bool> RequiredHashSet { get; set; } = null!;

        [TaskArgumentName("optional_single")]
        public bool OptionalSingle { get; set; }


        [TaskArgumentName("optional_list")] 
        public ImmutableList<bool> OptionalList { get; set; } = null!;

        [TaskArgumentName("optional_hashset")] 
        public ImmutableHashSet<bool> OptionalHashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<BoolTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
