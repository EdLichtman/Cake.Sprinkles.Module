using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    [TaskName("Default")]
    [TaskDescription($"The Default Description task")]
    public class DefaultTask : FrostingTask
    {
        [TaskArgumentName(nameof(DefaultDescribedValue))]
        [TaskArgumentDescription($"{nameof(DefaultTask)}{nameof(DefaultDescribedValue)}")]
        public string DefaultDescribedValue { get; set; } = null!;
    }
}
