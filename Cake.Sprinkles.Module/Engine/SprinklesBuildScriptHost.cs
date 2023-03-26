using Cake.Cli;
using Cake.Core.Diagnostics;
using Cake.Core;
using Cake.Frosting;
using Cake.Common;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Engine
{
    internal class SprinklesBuildScriptHost : BuildScriptHost<IFrostingContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildScriptHost"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="executionStrategy">The execution strategy.</param>
        /// <param name="context">The context.</param>
        /// <param name="reportPrinter">The report printer.</param>
        /// <param name="log">The log.</param>
        /// <param name="sprinklesTaskDescriptors">The <see cref="SprinklesTaskDescriptors"/>.</param>
        /// <param name="tasks">The <see cref="IEnumerable{IFrostingTask}"/>.</param>
        /// <param name="sprinklesDecorator">The <see cref="SprinklesDecorator"/>.</param>
        public SprinklesBuildScriptHost(
            ICakeEngine engine,
            IExecutionStrategy executionStrategy,
            IFrostingContext context,
            ICakeReportPrinter reportPrinter,
            ICakeLog log, 
            SprinklesTaskDescriptors sprinklesTaskDescriptors,
            IEnumerable<IFrostingTask> tasks,
            SprinklesDecorator sprinklesDecorator) : base(engine, executionStrategy, context, reportPrinter, log)
        {
            if (!(engine is SprinklesEngine))
            {
                log.Warning(SprinklesEngine.Warning_FrostingTeardownWillNotRun);
                var target = context.Argument("target", "Default");

                var executingTask = sprinklesTaskDescriptors.Discovered.FirstOrDefault(x => x.name == target);
                if (executingTask != (default, default, default))
                {
                    var task = tasks.First(x => SprinklesDecorations.GetTaskName(x) == target);
                    sprinklesDecorator.Decorate(task);
                }

            }
        }
    }
}
