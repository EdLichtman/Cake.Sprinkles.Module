using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to define a delimiter so that a string with this delimiter will be split into a list of values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentEnumerationDelimiterAttribute : Attribute
    {
        /// <summary>
        /// Gets the Delimiter, with which to separate your argument into multiple values.
        /// </summary>
        public string Delimiter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskArgumentEnumerationDelimiterAttribute"/> class.
        /// </summary>
        /// <param name="delimiter">the delimiter which, when appearing in a string, will separate your argument into multiple values.</param>
        public TaskArgumentEnumerationDelimiterAttribute(string delimiter)
        {
            Delimiter = delimiter;
        }
    }
}
