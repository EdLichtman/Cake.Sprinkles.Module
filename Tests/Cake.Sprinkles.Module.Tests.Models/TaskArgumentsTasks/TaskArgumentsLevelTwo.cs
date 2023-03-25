using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TaskArgumentsTasks
{
    public class TaskArgumentsLevelTwo
    {
        [TaskArgumentName(nameof(LevelTwoProperty))]
        public string LevelTwoProperty { get; set; } = null!;

        [TaskArguments]
        public TaskArgumentsLevelThree TaskArgumentsLevelThree { get; set; } = null!;
    }
}
