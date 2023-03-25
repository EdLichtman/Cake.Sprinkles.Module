using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using System.Collections.Immutable;

namespace Cake.Sprinkles.Module.Engine
{
    internal class SprinklesTaskDescriptors
    {
        public ImmutableList<(string name, string description,IFrostingTask task)> Discovered { get; set; }
        public SprinklesTaskDescriptors(IEnumerable<IFrostingTask> tasks)
        {
            var mutableInstance = new List<(string name, string description, IFrostingTask task)>();
            foreach(var task in tasks)
            {
                mutableInstance.Add(
                    (SprinklesDecorations.GetTaskName(task), 
                    SprinklesDecorations.GetTaskDescription(task), 
                    task));
            }
            
            Discovered = mutableInstance!.ToImmutableList();
        }
    }
}
