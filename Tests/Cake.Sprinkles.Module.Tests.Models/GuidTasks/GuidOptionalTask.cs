using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.GuidTasks
{
    [TaskName(nameof(GuidOptionalTask))]
    public class GuidOptionalTask : FrostingTask<SprinklesTestContext<GuidOptionalTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public Guid Single { get; set; }

        [TaskArgumentName(nameof(List))]
        public ImmutableList<Guid> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        public ImmutableHashSet<Guid> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<GuidOptionalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
