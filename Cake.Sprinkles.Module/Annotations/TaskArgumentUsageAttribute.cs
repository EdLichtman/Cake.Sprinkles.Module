using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TaskArgumentUsageAttribute : Attribute
    {
        public String UsageExample { get; set; }

        public TaskArgumentUsageAttribute(string usageExample)
        {
            UsageExample = usageExample;
        }
    }
}
