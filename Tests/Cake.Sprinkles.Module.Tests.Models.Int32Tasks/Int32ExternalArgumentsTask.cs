using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
namespace Cake.Sprinkles.Module.Tests.Models.Int32Tasks
{
    [TaskName(nameof(Int32ExternalArgumentsTask))]
    public class Int32ExternalArgumentsTask : FrostingTask<SprinklesTestContext<Int32ExternalArgumentsTask>>
    {
        [TaskArgumentName(nameof(Single))]
        public int Single { get; set; }

        [TaskArguments]
        public Int32ExternalArguments ExternalArguments { get; set; } = null!;
        public override void Run(SprinklesTestContext<Int32ExternalArgumentsTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
