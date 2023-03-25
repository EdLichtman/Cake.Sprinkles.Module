using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    public class DescriptionArgumentsChild
    {
        public const string Description = "Proof to show that children of taskArguments have everything about them described to the console.";

        [TaskArgumentName(nameof(ChildExternalArgument))]
        [TaskArgumentDescription(Description)]
        [TaskArgumentIsRequired]
        [TaskArgumentExampleValue("123")]
        public int ChildExternalArgument { get; set; }
    }
}
