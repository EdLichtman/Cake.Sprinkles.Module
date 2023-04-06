using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;

namespace Cake.Sprinkles.Module.Annotations
{
    internal class SprinklesDecorations
    {
        internal static string? GetNamespaceClassQualifiedPropertyName(PropertyInfo? propertyInfo)
        {
            if (propertyInfo == null)
            {
                return null;
            }

            var declaringType = propertyInfo.DeclaringType;
            if (declaringType == null)
            {
                return null;
            }

            return $"{declaringType.Namespace}.{declaringType.Name}.{propertyInfo.Name}";
        }

        internal static string GetTaskName(IFrostingTask task)
        {
            return GetTaskName(task.GetType());
        }

        internal static string GetTaskName(Type type)
        {
            var attr = type.GetCustomAttribute(typeof(TaskNameAttribute));
            if (attr != null)
            {
                return ((TaskNameAttribute)attr).Name;
            }

            return string.Empty;
        }

        internal static string GetTaskDescription(IFrostingTask task)
        {
            return GetTaskDescription(task.GetType());
        }

        internal static string GetTaskDescription(Type type)
        {
            var attr = type.GetCustomAttribute(typeof(TaskDescriptionAttribute));
            if (attr != null)
            {
                return ((TaskDescriptionAttribute)attr).Description;
            }

            return string.Empty;
        }

        internal static bool IsExternalTaskArguments(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(TaskArgumentsAttribute));
            if (attr != null)
            {
                return true;
            }

            return false;
        }

        internal static bool IsTaskArgument(PropertyInfo propertyInfo)
        {
            return !string.IsNullOrWhiteSpace(GetArgumentName(propertyInfo));
        }

        internal static string GetArgumentName(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(TaskArgumentNameAttribute));
            if (attr != null)
            {
                return ((TaskArgumentNameAttribute)attr).Name;
            }

            return string.Empty;
        }

        internal static string GetArgumentDescription(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(TaskArgumentDescriptionAttribute));
            if (attr != null)
            {
                return ((TaskArgumentDescriptionAttribute)attr).Description;
            }

            return string.Empty;
        }

        internal static Type GetArgumentType(PropertyInfo propertyInfo)
        {
            if (IsEnumeration(propertyInfo.PropertyType))
            {
                var enumeratedType = GetEnumeratedType(propertyInfo.PropertyType);
                if (enumeratedType == null)
                {
                    throw new SprinklesException(
                        propertyInfo,
                        "Something went wrong while getting argument type for argument. Expected an Enumeration, but found no enumerated type.",
                        $"Type: {propertyInfo.PropertyType}");
                }

                return propertyInfo.PropertyType.MakeGenericType(enumeratedType);
            }

            return propertyInfo.PropertyType;
        }

        internal static IList<string> GetArgumentExampleValues(PropertyInfo propertyInfo)
        {
            var values = new List<string>();
            var attrs = propertyInfo.GetCustomAttributes(typeof(TaskArgumentExampleValueAttribute))?.ToList();
            if (attrs?.Count > 0)
            {
                values.AddRange(attrs.Select(x => ((TaskArgumentExampleValueAttribute)x).UsageExample).ToList());
            }

            var typeConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType) as ITaskArgumentTypeConverter;
            var exampleValues = typeConverter?.GetExampleInputValues()?.ToList();
            if (exampleValues != null)
            {
                values.AddRange(exampleValues);
            }

            return values;
        }

        internal static string GetArgumentEnumerationDelimiterName(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(TaskArgumentEnumerationDelimiterAttribute));
            if (attr != null)
            {
                return ((TaskArgumentEnumerationDelimiterAttribute)attr).Delimiter;
            }

            return string.Empty;
        }

        internal static bool HasArgumentEnumerationDelimiter(PropertyInfo propertyInfo)
        {
            return !string.IsNullOrWhiteSpace(GetArgumentEnumerationDelimiterName(propertyInfo));
        }

        internal static bool IsFlag(PropertyInfo propertyInfo)
        {
            var nameAttribute = propertyInfo.GetCustomAttribute(typeof(TaskArgumentIsFlagAttribute));
            if (nameAttribute != null)
            {
                return ((TaskArgumentIsFlagAttribute)nameAttribute).IsFlag;
            }

            return false;
        }

        internal static bool IsRequired(PropertyInfo propertyInfo)
        {
            var nameAttribute = propertyInfo.GetCustomAttribute(typeof(TaskArgumentIsRequiredAttribute));
            if (nameAttribute != null)
            {
                return ((TaskArgumentIsRequiredAttribute)nameAttribute).IsRequired;
            }

            return false;
        }

        internal static IList<TaskArgumentValidationAttribute> GetArgumentValidations(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttributes().Where(x => x.GetType().IsAssignableTo(typeof(TaskArgumentValidationAttribute)));
            if (attr != null)
            {
                return attr.Select(x => (TaskArgumentValidationAttribute)x).ToList();
            }

            return new List<TaskArgumentValidationAttribute>();
        }

        internal static ITaskArgumentTypeConverter? GetArgumentConverter(PropertyInfo propertyInfo, IList<ITaskArgumentTypeConverter> converters)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(TaskArgumentConverterAttribute));
            if (attr != null)
            {
                var type = ((TaskArgumentConverterAttribute)attr).ConverterType;
                if (type != null)
                {
                    var converterTypes = converters.ToDictionary(x => x.GetType());
                    return converterTypes.ContainsKey(type) ? converterTypes[type] : null;
                }
            }

            return null;
        }

        internal static bool IsArgumentConverterValid(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(TaskArgumentConverterAttribute));
            if (attr != null)
            {
                return ((TaskArgumentConverterAttribute)attr).ConverterType != null;
            }

            return true;
        }

        internal static bool IsEnumeration(Type type)
        {
            return
                type.IsGenericType
                && (IsList(type) ||
                    IsSet(type));
        }

        /// <summary>
        /// If the given <paramref name="type"/> is an array or some other collection
        /// comprised of 0 or more instances of a "subtype", get that type
        /// </summary>
        /// <param name="type">the source type</param>
        /// <returns></returns>
        internal static Type? GetEnumeratedType(Type type)
        {
            return type.GetElementType() ?? (typeof(IEnumerable).IsAssignableFrom(type)
                ? type.GenericTypeArguments.FirstOrDefault()
                : null);
        }

        internal static bool IsList(Type type)
        {
            return typeof(ImmutableList<>) == type.GetGenericTypeDefinition();
        }

        internal static bool IsSet(Type type)
        {
            return typeof(ImmutableHashSet<>) == type.GetGenericTypeDefinition();
        }
    }
}
