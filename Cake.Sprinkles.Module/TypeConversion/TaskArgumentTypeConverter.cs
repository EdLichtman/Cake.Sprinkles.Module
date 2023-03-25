using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.TypeConversion
{
    /// <summary>
    /// The abstract <see cref="TaskArgumentTypeConverter{TType}"/>, assists in building a <see cref="ITaskArgumentTypeConverter"/>.
    /// </summary>
    /// <typeparam name="TType">The target type used for converting a string.</typeparam>
    public abstract class TaskArgumentTypeConverter<TType> : ITaskArgumentTypeConverter
    {
        public const string Message_CouldNotConvertToCustomType = "Could not convert to custom type '{0}.{1}'. Be sure the TasArgumentTypeConverter handles all cases.";
        /// <inheritdoc cref="ITaskArgumentTypeConverter.ConversionType"/>
        public Type ConversionType => typeof(TType);

        /// <summary>
        /// Converts a nullable string to a value of type <typeparamref name="TType"/>
        /// </summary>
        /// <param name="argument">The task argument value, from which you can get a single string or a list of strings.</param>
        /// <returns>An instance value of the type <typeparamref name="TType"/>.</returns>
        protected abstract TType ConvertType(TaskArgument argument);

        /// <summary>
        /// Converts a string into an instance of another type.
        /// </summary>
        /// <param name="argument">The task argument value, from which you can get a single string or a list of strings.</param>
        /// <returns>An instance of another type, created from the string.</returns>
        public object? Convert(TaskArgument argument)
        {
            try
            {
                // To make it more obvious to consumers of this library
                // that the return value is of the type you're converting to, this is only
                // a layer that lets you tightly type in the TType.
                return ConvertType(argument);
            } 
            catch (Exception ex) 
            {
                throw new SprinklesCaptureException(
                    ex, 
                    GetType(), 
                    string.Format(
                        Message_CouldNotConvertToCustomType, 
                        typeof(TType).Namespace,
                        typeof(TType).Name));
            }
            
        }

        /// <summary>
        /// Gets the Usage Values for Usage Descriptions. No need to include --argument_name=. Only include values.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{string}"/> of Usage Values.</returns>
        public virtual IEnumerable<string> GetExampleInputValues()
        {
            yield break;
        }
    }
}
