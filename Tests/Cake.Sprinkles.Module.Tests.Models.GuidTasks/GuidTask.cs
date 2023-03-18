using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.GuidTasks
{
    [TaskName("Default")]
    public class GuidTask : FrostingTask<SprinklesTestContext<GuidTask>>
    {
        [TaskArgumentName("required_single")]
        [TaskArgumentIsRequired]
        public Guid RequiredSingle { get; set; }

        [TaskArgumentName("required_list")]
        [TaskArgumentIsRequired]
        public ImmutableList<Guid> RequiredList { get; set; } = null!;

        [TaskArgumentName("required_hashset")]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<Guid> RequiredHashSet { get; set; } = null!;

        [TaskArgumentName("optional_single")]
        public Guid? OptionalSingle { get; set; }


        [TaskArgumentName("optional_list")] 
        public ImmutableList<Guid> OptionalList { get; set; } = null!;

        [TaskArgumentName("optional_hashset")]
        public ImmutableHashSet<Guid> OptionalHashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<GuidTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
