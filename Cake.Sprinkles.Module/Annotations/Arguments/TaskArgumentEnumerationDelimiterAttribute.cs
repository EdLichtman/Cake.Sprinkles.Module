using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentEnumerationDelimiterAttribute : Attribute
    {
        public string Delimiter { get; }

        public TaskArgumentEnumerationDelimiterAttribute(string delimiter)
        {
            Delimiter = delimiter;
        }
    }
}
