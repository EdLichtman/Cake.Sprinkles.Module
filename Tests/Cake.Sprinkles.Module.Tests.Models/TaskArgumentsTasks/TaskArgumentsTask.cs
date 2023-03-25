using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TaskArgumentsTasks
{
    [TaskName(nameof(TaskArgumentsTask))]
    public class TaskArgumentsTask : FrostingTask<SprinklesTestContext<TaskArgumentsTask>>
    {
        [TaskArguments]
        public TaskArgumentsLevelOne TaskArgumentsLevelOne { get; set; } = null!;

        public override void Run(SprinklesTestContext<TaskArgumentsTask> context)
        {
            context.Task = this;
        }
    }
}
