using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TaskArgumentExampleValueAttribute : Attribute
    {
        public string UsageExample { get; set; }

        public TaskArgumentExampleValueAttribute(string usageExample)
        {
            UsageExample = usageExample;
        }
    }
}
