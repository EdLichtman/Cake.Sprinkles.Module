using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.Int32
{
    [TaskName("Default")]
    public class Int32Task : FrostingTask<SprinklesTestContext<Int32Task>>
    {
        [TaskArgumentName("required_single")]
        [TaskArgumentIsRequired]
        public int RequiredSingle { get; set; }

        [TaskArgumentName("required_list")]
        [TaskArgumentIsRequired]
        public ImmutableList<int> RequiredList { get; set; } = null!;

        [TaskArgumentName("required_hashset")]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<int> RequiredHashSet { get; set; } = null!;

        [TaskArgumentName("optional_single")]
        public int OptionalSingle { get; set; }


        [TaskArgumentName("optional_list")]
        public ImmutableList<int> OptionalList { get; set; } = null!;

        [TaskArgumentName("optional_hashset")]
        public ImmutableHashSet<int> OptionalHashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<Int32Task> testContext)
        {
            testContext.Task = this;
        }
    }
}
