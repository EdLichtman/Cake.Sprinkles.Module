using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    [TaskName(nameof(WithDependencyTask))]
    [IsDependentOn(typeof(DependencyTask))]
    public class WithDependencyTask : FrostingTask
    {
    }

    [TaskName(nameof(DependencyTask))]
    public class DependencyTask : FrostingTask
    {

    }

    [TaskName(nameof(WithDependeeTask))]
    public class WithDependeeTask : FrostingTask
    {

    }

    [TaskName(nameof(DependeeTask))]
    [IsDependeeOf(typeof(WithDependeeTask))]
    public class DependeeTask : FrostingTask
    {

    }
}
