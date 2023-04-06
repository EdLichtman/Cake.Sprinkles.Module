using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;
using System.ComponentModel;
using System.Globalization;

namespace Cake.Sprinkles.Module.TypeConversion
{
    /// <summary>
    /// The abstract <see cref="TaskArgumentTypeConverter{TType}"/>, assists in building a <see cref="ITaskArgumentTypeConverter"/>.
    /// </summary>
    /// <typeparam name="TType">The target type used for converting a string.</typeparam>
    public abstract class TaskArgumentTypeConverter<TType> : TypeConverter, ITaskArgumentTypeConverter
    {
        /// <summary>
        /// Gets the target type for which to convert a string argument.
        /// </summary>
        public Type ConversionType => typeof(TType);

        /// <summary>
        /// Converts a nullable string to a value of type <typeparamref name="TType"/>
        /// </summary>
        /// <param name="argument">The task argument value, from which you can get a single string or a list of strings.</param>
        /// <param name="cultureInfo">The <see cref="CultureInfo"/> of the application.</param>
        /// <returns>An instance value of the type <typeparamref name="TType"/>.</returns>
        protected abstract TType ConvertType(TaskArgument argument, CultureInfo? cultureInfo);

        /// <summary>
        /// Receives a value indicating whether this <see cref="TypeConverter"/> can convert to a <see cref="TaskArgument"/> from our <see cref="Type"/>.
        /// </summary>
        /// <param name="context">The <see cref="ITypeDescriptorContext"/>.</param>
        /// <param name="targetType">The target <see cref="Type"/></param>
        /// <returns>A value indicating whether this <see cref="TypeConverter"/> can convert to a <see cref="TaskArgument"/>.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? targetType)
        {
            return false;
        }

        /// <summary>
        /// Receives a value indicating whether this <see cref="TypeConverter"/> can convert from a particular <see cref="Type"/>.
        /// </summary>
        /// <param name="context">The <see cref="ITypeDescriptorContext"/>.</param>
        /// <param name="sourceType">The <see cref="Type"/> to convert from.</param>
        /// <returns>A value indicating whether this <see cref="TypeConverter"/> can convert from a particular <see cref="Type"/>.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(TaskArgument);
        }

        /// <summary>
        /// Converts from a <see cref="TaskArgument"/> into a new type.
        /// </summary>
        /// <param name="context">The <see cref="ITypeDescriptorContext"/>.</param>
        /// <param name="cultureInfo">The <see cref="CultureInfo"/>.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="SprinklesCaptureException"></exception>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? cultureInfo, object value)
        {
            try
            {
                if (value is TaskArgument argument)
                {
                    return ConvertType(argument, cultureInfo);
                }

                throw new Exception(
                    "Value passed in was not TaskArgument. This should not be the case. Please open an issue with exact steps to reproduce so we can fix this.");
            }
            catch (Exception ex)
            {
                throw new SprinklesCaptureException(
                    ex,
                    GetType(),
                    string.Format(
                        SprinklesValidator.Message_ArgumentConverterCouldNotConvertToCustomType,
                        typeof(TType).Namespace,
                        typeof(TType).Name));
            }
        }

        /// <summary>
        /// Gets the Usage Values for Usage Descriptions. No need to include --argument_name=. Only include values.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{String}"/> of Usage Values.</returns>
        public virtual IEnumerable<string> GetExampleInputValues()
        {
            yield break;
        }
    }
}
