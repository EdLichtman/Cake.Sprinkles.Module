using System.Collections.Immutable;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Tests.Models.String
{
    [TaskName("Default")]
    public class StringTask : FrostingTask<SprinklesTestContext<StringTask>>
    {
        [TaskArgumentName("required_single")]
        [TaskArgumentIsRequired]
        public string RequiredSingle { get; set; } = null!;

        [TaskArgumentName("required_list")]
        [TaskArgumentIsRequired]
        public ImmutableList<string> RequiredList { get; set; } = null!;

        [TaskArgumentName("required_hashset")]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<string> RequiredHashSet { get; set; } = null!;

        [TaskArgumentName("optional_single")]
        public string? OptionalSingle { get; set; }


        [TaskArgumentName("optional_list")] 
        public ImmutableList<string> OptionalList { get; set; } = null!;

        [TaskArgumentName("optional_hashset")]
        public ImmutableHashSet<string> OptionalHashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<StringTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
