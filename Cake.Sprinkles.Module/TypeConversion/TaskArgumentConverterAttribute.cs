using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.TypeConversion
{
    /// <summary>
    /// Allows you to Specify which TaskArgumentTypeConverter can be used on a particular Task Argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TaskArgumentConverterAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the <see cref="ITaskArgumentTypeConverter"/>.
        /// </summary>
        public Type? ConverterType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskArgumentConverterAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the <see cref="ITaskArgumentTypeConverter"/>.</param>
        public TaskArgumentConverterAttribute(Type type)
        {
            if (type.IsAssignableTo(typeof(ITaskArgumentTypeConverter)))
            {
                ConverterType = type;
            }
        }
    }
}
