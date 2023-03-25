using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.Tests.Models;

namespace Cake.Sprinkles.Module.Tests.Models.BooleanTasks
{
    [TaskName(nameof(BoolFlagTask))]
    public class BoolFlagTask : FrostingTask<SprinklesTestContext<BoolFlagTask>>
    {
        [TaskArgumentName(nameof(HasArgument))]
        [TaskArgumentIsFlag]
        public bool HasArgument { get; set; }

        public override void Run(SprinklesTestContext<BoolFlagTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
