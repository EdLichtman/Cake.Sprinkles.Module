﻿using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TaskArgumentsTasks
{
    public class TaskArgumentsLevelThree
    {
        [TaskArgumentName(nameof(LevelThreeProperty))]
        public string LevelThreeProperty { get; set; } = null!;
    }
}
