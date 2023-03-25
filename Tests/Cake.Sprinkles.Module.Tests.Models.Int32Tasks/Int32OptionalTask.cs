using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Int32Tasks
{
    [TaskName(nameof(Int32OptionalTask))]
    public class Int32OptionalTask : FrostingTask<SprinklesTestContext<Int32OptionalTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public int Single { get; set; }

        [TaskArgumentName(nameof(List))]
        public ImmutableList<int> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        public ImmutableHashSet<int> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<Int32OptionalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
