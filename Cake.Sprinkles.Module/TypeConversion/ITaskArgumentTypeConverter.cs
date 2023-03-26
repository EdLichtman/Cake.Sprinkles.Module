using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.TypeConversion
{
    /// <summary>
    /// An interface that allows you to identify custom ways to convert your Task Argument String(s) into a new value. 
    /// </summary>
    /// <remarks>
    /// Prefer to use the <see cref="TaskArgumentTypeConverter{TType}"/> class instead.
    /// </remarks>
    public interface ITaskArgumentTypeConverter
    {
        /// <summary>
        /// Gets the target type for which to convert a string argument.
        /// </summary>
        public Type ConversionType { get; }

        /// <summary>
        /// Gets the Usage Values for Usage Descriptions. No need to include --argument_name=. Only include values.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{String}"/> of Usage Values.</returns>
        public IEnumerable<string> GetExampleInputValues();

        /// <summary>
        /// Converts a string into an instance of another type.
        /// </summary>
        /// <param name="argument">The task argument value, from which you can get a single string or a list of strings.</param>
        /// <returns>An instance of another type, created from the string.</returns>
        public object? Convert(TaskArgument argument);
    }
}
