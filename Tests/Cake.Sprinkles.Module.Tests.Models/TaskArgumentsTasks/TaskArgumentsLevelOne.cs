using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TaskArgumentsTasks
{
    public class TaskArgumentsLevelOne
    {
        [TaskArgumentName(nameof(LevelOneProperty))]
        public string LevelOneProperty { get; set; } = null!;

        [TaskArgumentName(nameof(LevelOneIntProperty))]
        public int LevelOneIntProperty { get; set; }

        [TaskArguments]
        public TaskArgumentsLevelTwo TaskArgumentsLevelTwo { get; set; } = null!;
    }
}
