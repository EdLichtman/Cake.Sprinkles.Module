using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Tests.Models;

namespace Cake.Sprinkles.Module.Tests.Models.BooleanTasks
{
    [TaskName("Flag")]
    public class BoolFlagTask : FrostingTask<SprinklesTestContext<BoolFlagTask>>
    {
        [TaskArgumentName("flag")]
        [TaskArgumentIsFlag]
        public bool HasArgument { get; set; }

        public override void Run(SprinklesTestContext<BoolFlagTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
