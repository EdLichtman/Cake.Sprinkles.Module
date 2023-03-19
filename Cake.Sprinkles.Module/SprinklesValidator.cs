using Cake.Core;
using Cake.Sprinkles.Module.Annotations;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace Cake.Sprinkles.Module
{
    internal static class SprinklesValidator
    {
        public const string Message_EnumerableMustImplementImmutableList = $"An enumerable Argument must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}";
        public const string Message_FlagMustNotBeEnumerable = "An argument that accepts a flag can only contain one value.";
        public const string Message_EnumerableDelimiterMustImplementEnumerable = $"An Argument with an enumerable delimiter must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}";
        public const string Message_FlagMustBeBoolean = "An argument that accepts a flag must be a boolean.";
        public const string Message_FlagCannotBeRequired = "An argument that accepts a flag cannot be a required argument.";
        public const string Message_EnumerableDelimiterCannotHaveMoreThanOneArgument = "An argument that allows an enumeration delimiter must contain only one argument input. Any further inputs must be separated by a delimiter.";
        public static void ValidateBuildClassProperty(PropertyInfo property)
        {
            var name = SprinklesDecorations.GetArgumentName(property);
            var isFlag = SprinklesDecorations.IsFlag(property);
            var isRequired = SprinklesDecorations.IsRequired(property);
            var hasDelimiter = !String.IsNullOrWhiteSpace(SprinklesDecorations.GetArgumentEnumerationDelimiterName(property));

            var enumeratedType = SprinklesDecorations.GetEnumeratedType(property.PropertyType);
            var isEnumeration = SprinklesDecorations.IsEnumeration(property.PropertyType);

            if (!isEnumeration)
            {
                if (enumeratedType != null)
                {
                    throw new SprinklesException(
                        property,
                        Message_EnumerableMustImplementImmutableList);
                }

                if (hasDelimiter)
                {
                    throw new SprinklesException(
                        property,
                        Message_EnumerableDelimiterMustImplementEnumerable);
                }
            }

            if (isFlag)
            {
                if (property.PropertyType != typeof(bool))
                {
                    if (SprinklesDecorations.IsEnumeration(property.PropertyType))
                    {
                        throw new SprinklesException(
                            property,
                            Message_FlagMustNotBeEnumerable);
                    }

                    throw new SprinklesException(
                        property,
                        Message_FlagMustBeBoolean);
                }


                if (isRequired)
                    throw new SprinklesException(
                        property,
                        Message_FlagCannotBeRequired);
            }
        }

        public static void ValidateEnumerableRuntimeArguments(object? values, PropertyInfo property)
        {
            if (values is ICollection {Count: > 1} && SprinklesDecorations.HasArgumentEnumerationDelimiter(property))
            {
                throw new SprinklesException(
                    property,
                    Message_EnumerableDelimiterCannotHaveMoreThanOneArgument,
                    $"Delimiter: {SprinklesDecorations.GetArgumentEnumerationDelimiterName(property)}");
            }
        }
    }
}
