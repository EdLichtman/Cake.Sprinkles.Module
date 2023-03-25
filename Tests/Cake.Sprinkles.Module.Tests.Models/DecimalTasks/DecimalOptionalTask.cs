using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DecimalTasks
{
    [TaskName(nameof(DecimalOptionalTask))]
    public class DecimalOptionalTask : FrostingTask<SprinklesTestContext<DecimalOptionalTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public decimal Single { get; set; }

        [TaskArgumentName(nameof(List))]
        public ImmutableList<decimal> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        public ImmutableHashSet<decimal> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<DecimalOptionalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
