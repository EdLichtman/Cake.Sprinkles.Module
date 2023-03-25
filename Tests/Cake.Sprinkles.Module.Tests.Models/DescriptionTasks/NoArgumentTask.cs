using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    [TaskName(nameof(NoArgumentTask))]
    [TaskDescription($"{nameof(TaskDescriptionAttribute)}{nameof(NoArgumentTask)}")]
    public class NoArgumentTask : FrostingTask
    {
        // no Arguments text should appear.
    }
}
