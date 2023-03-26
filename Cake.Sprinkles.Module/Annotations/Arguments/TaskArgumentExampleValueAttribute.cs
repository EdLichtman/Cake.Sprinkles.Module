using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to describe how to use a task argument through an example value.
    /// You can use multiple of these attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TaskArgumentExampleValueAttribute : Attribute
    {
        /// <summary>
        /// Gets the example of how to use the Task Argument.
        /// </summary>
        public string UsageExample { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskArgumentExampleValueAttribute"/> class.
        /// </summary>
        /// <param name="usageExample">The example value that should be in place of the {0} placeholder in: --ArgumentName={0}</param>
        public TaskArgumentExampleValueAttribute(string usageExample)
        {
            UsageExample = usageExample;
        }
    }
}
