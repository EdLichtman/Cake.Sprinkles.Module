using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DateTimeTasks
{
    [TaskName(nameof(DateTimeOptionalTask))]
    public class DateTimeOptionalTask : FrostingTask<SprinklesTestContext<DateTimeOptionalTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public DateTime Single { get; set; }

        [TaskArgumentName(nameof(List))]
        public ImmutableList<DateTime> List { get; set; } = null!;

        [TaskArgumentName(nameof(HashSet))]
        public ImmutableHashSet<DateTime> HashSet { get; set; } = null!;

        public override void Run(SprinklesTestContext<DateTimeOptionalTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
