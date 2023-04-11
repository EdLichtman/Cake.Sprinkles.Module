using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.Tests.Models.TypeConversion;
using Cake.Sprinkles.Module.TypeConversion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    [TaskName(nameof(CustomTypeTask))]
    [TaskDescription($"{nameof(TaskDescriptionAttribute)}{nameof(CustomTypeTask)}")]
    public class CustomTypeTask : FrostingTask
    {
        [TaskArgumentName(nameof(CustomType))]
        [TaskArgumentConverter(typeof(TypeWithUsageConverter))]
        [TaskArgumentIsRequired]
        public TypeWithUsage CustomType { get; set; } = null!;
    }
}
