using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Cake.Frosting;

namespace Cake.Sprinkles.Module.Annotations
{
    internal class SprinklesDecorations
    {
        internal static String GetTaskName(IFrostingTask task)
        {
            var type = task.GetType();
            var nameAttribute = type.GetCustomAttribute(typeof(TaskNameAttribute));
            if (nameAttribute != null)
            {
                return ((TaskNameAttribute)nameAttribute).Name;
            }

            return String.Empty;
        }

        internal static String GetTaskDescription(IFrostingTask task)
        {
            var type = task.GetType();
            var nameAttribute = type.GetCustomAttribute(typeof(TaskDescriptionAttribute));
            if (nameAttribute != null)
            {
                return ((TaskDescriptionAttribute)nameAttribute).Description;
            }

            return String.Empty;
        }

        internal static bool IsTaskArgument(PropertyInfo propertyInfo)
        {
            return !string.IsNullOrWhiteSpace(GetArgumentName(propertyInfo));
        }

        internal static string GetArgumentName(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute(typeof(TaskArgumentNameAttribute));
            if (attribute != null)
            {
                return ((TaskArgumentNameAttribute)attribute).Name;
            }

            return string.Empty;
        }

        internal static string GetArgumentDescription(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute(typeof(TaskArgumentDescriptionAttribute));
            if (attribute != null)
            {
                return ((TaskArgumentDescriptionAttribute)attribute).Description;
            }

            return string.Empty;
        }

        internal static IList<string> GetArgumentUsageExamples(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(TaskArgumentUsageAttribute))?.ToList();
            if (attributes?.Count > 0)
            {
                return attributes.Select(x => ((TaskArgumentUsageAttribute)x).UsageExample).ToList();
            }

            return new List<string>();
        }

        internal static string GetArgumentEnumerationDelimiterName(PropertyInfo propertyInfo)
        {
            var nameAttribute = propertyInfo.GetCustomAttribute(typeof(TaskArgumentEnumerationDelimiterAttribute));
            if (nameAttribute != null)
            {
                return ((TaskArgumentEnumerationDelimiterAttribute)nameAttribute).Delimiter;
            }

            return string.Empty;
        }

        internal static bool HasArgumentEnumerationDelimiter(PropertyInfo propertyInfo)
        {
            return !String.IsNullOrWhiteSpace(GetArgumentEnumerationDelimiterName(propertyInfo));
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
