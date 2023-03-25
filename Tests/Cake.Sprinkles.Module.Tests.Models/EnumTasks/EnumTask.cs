using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.EnumTasks
{
    [TaskName(nameof(EnumTask))]
    public class EnumTask : FrostingTask<SprinklesTestContext<EnumTask>>
    {
        [TaskArgumentName(nameof(TestEnum))]
        public TestEnum TestEnum { get; set; }

        public override void Run(SprinklesTestContext<EnumTask> context)
        {
            context.Task = this;
        }
    }
}
