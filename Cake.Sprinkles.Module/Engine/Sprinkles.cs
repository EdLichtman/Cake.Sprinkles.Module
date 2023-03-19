using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module.Engine
{
    internal class Sprinkles
    {
        public static TTask Decorate<TTask>(
            TTask frostingTask,
            ITaskSetupContext context)
            where TTask : IFrostingTask
        {
            var properties =
                frostingTask
                    .GetType()
                    .GetProperties()
                    .Where(SprinklesDecorations.IsTaskArgument).ToList();

            var exceptions = new List<SprinklesException>();

            foreach (var property in properties)
            {
                try
                {
                    var name = SprinklesDecorations.GetArgumentName(property);

                    SprinklesValidator.ValidateBuildClassProperty(property);

                    object? arguments;
                    var isEnumeration = SprinklesDecorations.IsEnumeration(property.PropertyType);

                    if (isEnumeration)
                    {
                        arguments = GetArguments(context, property);
                    }
                    else
                    {
                        arguments = GetArgument(context, property);
                        if (arguments == null && SprinklesDecorations.IsFlag(property) && property.PropertyType == typeof(bool))
                        {
                            arguments = context.Arguments.HasArgument(name);
                        }
                    }

                    if (SprinklesDecorations.IsRequired(property))
                    {
                        if (arguments == null || arguments is ICollection { Count: 0 })
                        {
                            var message = $"Argument '{name}' was not set";
                            throw new SprinklesException(property, message);
                        }
                    }

                    property.SetValue(frostingTask, arguments);
                }
                catch (SprinklesException exception)
                {
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return frostingTask;
        }

        

        private static object? GetArgument(ITaskSetupContext context, PropertyInfo property)
        {
            var template = SprinklesDecorations.GetArgumentName(property);
            var type = property.PropertyType;
            try
            {
                var value = MethodGetArgumentFromCakeContext!.Invoke(null, new object[] { context.Arguments, template });
                if (value != null)
                {
                    return Parse(property, value.ToString()!, type);
                }

                value = GetMethodGetConfigurationValueFromCakeConfiguration().Invoke(context.Configuration,
                        new object[] {template});

                if (value != null)
                {
                    return Parse(property, value.ToString()!, type);
                }

                return null;
            }
            catch (TargetInvocationException e)
            {
                throw new SprinklesException(property, e.InnerException!.Message);
            }

        }

        private static object GetArguments(ITaskSetupContext context, PropertyInfo property)
        {
            var template = SprinklesDecorations.GetArgumentName(property);
            var type = property.PropertyType;
            var enumeratedType = SprinklesDecorations.GetEnumeratedType(property.PropertyType)!;
            var delimiter = SprinklesDecorations.GetArgumentEnumerationDelimiterName(property);
            var hasDelimiter = !String.IsNullOrWhiteSpace(delimiter);

            try
            {
                // At this point it's a collection of strings
                var values = GetMethodGetArgumentsAsCollectionFromCakeContext().Invoke(context.Arguments, new object[] { template })!;

                SprinklesValidator.ValidateEnumerableRuntimeArguments(values, property);

                // Here we get it as a List of strings so we can perform operations on it.
                var list = (List<string>)GetMethodEnumerableToList(typeof(string)).Invoke(null, new object[] { values })!;

                if (list.Count == 0)
                {
                    var configuration =  GetMethodGetConfigurationValueFromCakeConfiguration().Invoke(context.Configuration, new object[] {template});

                    if (!string.IsNullOrWhiteSpace(configuration as string))
                    {
                        list.Add((string)configuration);
                    }
                }

                if (list.Count == 1 && hasDelimiter)
                {
                    var value = list.First();
                    values = value.Split(delimiter).ToList();
                }
                else
                {
                    values = list;
                }

                var typedValues = Cast(values, property);

                if (SprinklesDecorations.IsList(type))
                {
                    return ToImmutableList(typedValues, enumeratedType);
                }

                return ToImmutableHashSet(typedValues, enumeratedType);
            }
            catch (TargetInvocationException e)
            {
                throw new SprinklesException(property, e.InnerException!.Message);
            }
        }

        private static object Cast(object values, PropertyInfo property)
        {
            var enumeratedType = SprinklesDecorations.GetEnumeratedType(property.PropertyType)!;

            var validValues =
                ((IEnumerable<object?>)values)
                .Where(value => value != null)
                .Select(value => Parse(property, value!.ToString()!, enumeratedType));

            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast), new[] { typeof(IEnumerable) });
            var castGenericInvocation = castMethod!.MakeGenericMethod(enumeratedType);

            try
            {
                var castValue = castGenericInvocation.Invoke(null, new object?[] { validValues })!;

                return castValue;
            }
            catch (TargetInvocationException e)
            {
                throw new SprinklesException(property, e.InnerException!.Message);
            }

        }

        private static object ToImmutableList(object value, Type enumeratedType)
        {
            var asImmutableListMethod =
                typeof(ImmutableList)
                    .GetMethods()
                    .Where(method => method.Name == nameof(ImmutableList.CreateRange))
                    .First(method => method.GetParameters().Length == 1)
                    .MakeGenericMethod(enumeratedType);

            return asImmutableListMethod.Invoke(null, new[] { value })!;
        }

        private static object ToImmutableHashSet(object value, Type enumeratedType)
        {
            var asImmutableHashSetMethod =
                typeof(ImmutableHashSet)
                    .GetMethods()
                    .Where(method => method.Name == nameof(ImmutableHashSet.CreateRange))
                    .First(method => method.GetParameters().Length == 1)
                    .MakeGenericMethod(enumeratedType);

            return asImmutableHashSetMethod.Invoke(null, new[] { value })!;
        }

        /// <summary>
        /// Converts a string value into the type specified.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The string value.</param>
        /// <param name="type">The type to convert it to.</param>
        /// <returns>An object of the base type specified.</returns>
        /// <exception cref="SprinklesException">If the type cannot be cast.</exception>
        private static object Parse(PropertyInfo property, string value, Type type)
        {
            var exceptionMessage = $"Argument was not a valid value for type '{type.Name}'";
            var additionalInfo = $"Argument: {value}";
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                return converter.ConvertFromInvariantString(value)!;
            }
            catch (Exception)
            {
                var constructor = type.GetConstructors()
                    .FirstOrDefault(x => x.GetParameters().Length == 1 && !x.ContainsGenericParameters);
                if (constructor != null)
                {
                    return constructor.Invoke(new object[] {value});
                }

                throw new SprinklesException(property, exceptionMessage, additionalInfo);
            }
            
        }

        private static MethodInfo MethodGetArgumentFromCakeContext =>
            typeof(CakeArgumentsExtensions).GetMethods()
                .First(method => method.Name == nameof(CakeArgumentsExtensions.GetArgument));

        private static MethodInfo GetMethodGetArgumentsAsCollectionFromCakeContext() =>
            typeof(ICakeArguments)
                .GetMethods()
                .Where(method => method.Name == nameof(ICakeArguments.GetArguments))
                .First(method => method.GetParameters().Length == 1)!;

        private static MethodInfo GetMethodGetConfigurationValueFromCakeConfiguration() =>
            typeof(ICakeConfiguration)
                .GetMethods()
                .First(method => method.Name == nameof(ICakeConfiguration.GetValue))!;

        private static MethodInfo GetMethodEnumerableToList(Type enumeratedType) =>
            typeof(Enumerable)
                .GetMethods()
                .First(method => method.Name == nameof(Enumerable.ToList))
                .MakeGenericMethod(enumeratedType);
    }
}
