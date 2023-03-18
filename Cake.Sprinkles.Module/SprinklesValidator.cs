using Cake.Core;
using Cake.Sprinkles.Module.Annotations;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace Cake.Sprinkles.Module
{
    internal static class SprinklesValidator
    {
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
                        $"An enumerable Argument must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}");
                }

                if (hasDelimiter)
                {
                    throw new SprinklesException(
                        property,
                        $"An Argument with an enumerable delimiter must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}");
                }
            }

            if (isFlag)
            {
                if (property.PropertyType != typeof(bool))
                    throw new SprinklesException(
                        property,
                        $"An argument that accepts a flag must be a boolean.");

                if (isRequired)
                    throw new SprinklesException(
                        property,
                        $"An argument that accepts a flag cannot be a required argument.");
            }
        }

        public static void ValidateEnumerableRuntimeArguments(object? values, PropertyInfo property)
        {
            if (values is ICollection {Count: > 1} && SprinklesDecorations.HasArgumentEnumerationDelimiter(property))
            {
                throw new SprinklesException(
                    property,
                    "An argument that allows an enumeration delimiter must contain only one argument input. Any further inputs must be separated by a delimiter.",
                    $"Delimiter: {SprinklesDecorations.GetArgumentEnumerationDelimiterName(property)}");
            }
        }
    }
}
