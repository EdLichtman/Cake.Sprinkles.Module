using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to offload the TaskArgument parsing to a child class, so that you can share arguments between tasks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentsAttribute : Attribute
    {
    }
}
