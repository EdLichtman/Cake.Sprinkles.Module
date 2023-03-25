using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.InvalidTasks
{
    public class ParentInvalidChildArguments
    {
        [TaskArgumentName(nameof(TestForParentOwnership))]
        public string TestForParentOwnership { get; set; } = null!;
    }
}
