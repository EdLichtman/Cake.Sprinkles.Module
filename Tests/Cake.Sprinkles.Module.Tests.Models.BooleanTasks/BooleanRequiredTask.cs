using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.BooleanTasks
{
    [TaskName(nameof(BooleanRequiredTask))]
    public class BooleanRequiredTask : FrostingTask<SprinklesTestContext<BooleanRequiredTask>>
    {
        [TaskArgumentName(nameof(Single))]
        [TaskArgumentIsRequired]
        public bool Single { get; set; }

        [TaskArgumentName(nameof(List))]
        [TaskArgumentIsRequired]
        public ImmutableList<bool> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<bool> HashSet { get; set; } = null!;


        public override void Run(SprinklesTestContext<BooleanRequiredTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
