using Cake.Frosting;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Annotations
{
    internal class SprinklesArgumentsProvider
    {
        private readonly SprinklesValidator _validator;
        public SprinklesArgumentsProvider(SprinklesValidator validator)
        {
            _validator = validator;
        }

        internal IList<(PropertyInfo property, object? parent)> GetAllArguments(IFrostingTask frostingTask, IList<SprinklesException> exceptions)
        {
            // Find any Child Arguments that are already invalid at compile-time
            InstantiateDecoratedArguments(frostingTask, exceptions);

            // remove those, so that we don't include any children of the invalid properties
            var propertiesWithExceptions = exceptions.Select(x => x.PropertyInfo).ToList();

            var argumentsOfTask =
                frostingTask!
                    .GetType()
                    .GetProperties()
                    .Where(SprinklesDecorations.IsTaskArgument)
                    .Where(x => !SprinklesDecorations.IsExternalTaskArguments(x))
                    .Select(prop => (property: prop, parent: (object?)frostingTask)).ToList();

            argumentsOfTask.AddRange(GetExternalTaskArguments(frostingTask.GetType(), frostingTask, propertiesWithExceptions));

            return argumentsOfTask
                .Where(x => !SprinklesDecorations.IsExternalTaskArguments(x.property))
                .ToList();
        }

        private void InstantiateDecoratedArguments(object frostingTask, IList<SprinklesException> exceptions)
        {
            var externalArgumentProperties =
                frostingTask
                    .GetType()
                    .GetProperties()
                    .Where(SprinklesDecorations.IsExternalTaskArguments)
                    .ToList();

            foreach (var property in externalArgumentProperties)
            {
                try
                {
                    _validator.ValidateBuildClassProperty(property);

                    property.SetValue(frostingTask, Activator.CreateInstance(property.PropertyType));
                    InstantiateDecoratedArguments(property.GetValue(frostingTask)!, exceptions);
                }
                catch (SprinklesException ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        private static IEnumerable<(PropertyInfo property, object? parent)> GetExternalTaskArguments(Type type, object? current, IList<PropertyInfo>? exceptProperties = null)
        {
            if (exceptProperties == null)
            {
                exceptProperties = new List<PropertyInfo>();
            }

            var properties = type.GetProperties().Where(SprinklesDecorations.IsExternalTaskArguments);

            foreach (var property in properties.Except(exceptProperties))
            {
                var child = property.GetValue(current);
                if (child != null)
                {
                    yield return (property, current);

                    foreach (var externalProperty in GetExternalTaskArgumentsRecursive(property, child, exceptProperties))
                    {
                        yield return (externalProperty.property, externalProperty.parent);
                    }
                }
            }

        }

        private static IEnumerable<(PropertyInfo property, object? parent, object? child)> GetExternalTaskArgumentsRecursive(PropertyInfo parentProperty, object? current, IList<PropertyInfo> exceptProperties)
        {
            var type = parentProperty.PropertyType;
            var properties = type.GetProperties();
            foreach (var property in properties.Except(exceptProperties))
            {
                var isExternalTaskArguments = SprinklesDecorations.IsExternalTaskArguments(property);
                var child = property.GetValue(current);

                if (child == null && isExternalTaskArguments)
                {
                    continue;
                }

                yield return (property, current, child);
                if (isExternalTaskArguments)
                {
                    foreach (var recursiveArgument in GetExternalTaskArgumentsRecursive(property, child, exceptProperties))
                    {
                        yield return recursiveArgument;
                    }
                }
            }
        }
    }
}
