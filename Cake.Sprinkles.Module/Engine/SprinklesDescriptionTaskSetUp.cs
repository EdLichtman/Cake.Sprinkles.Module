using Cake.Core;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Engine;

public class SprinklesDescriptionTaskSetUp : IFrostingTaskSetup
{
    private IDictionary<string, IFrostingTask> _tasks;

    public SprinklesDescriptionTaskSetUp(IEnumerable<IFrostingTask> tasks)
    {
        _tasks = tasks.ToDictionary(SprinklesDecorations.GetTaskName);
    }
    public void Setup(ICakeContext context, ITaskSetupContext info)
    {
        if (_tasks.TryGetValue(info.Task.Name, out var task))
        {
            Sprinkles.Decorate(task, info);
        }
    }
}