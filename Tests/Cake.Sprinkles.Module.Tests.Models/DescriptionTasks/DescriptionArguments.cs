using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    public class DescriptionArguments
    {
        [TaskArgumentName(nameof(ExternalArgument))]
        public string ExternalArgument { get; set; } = null!;

        [TaskArguments]
        public DescriptionArgumentsChild Arguments { get; set; } = null!;
    }
}
