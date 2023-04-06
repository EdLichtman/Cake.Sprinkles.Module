using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;
using Newtonsoft.Json.Linq;

namespace Cake.Sprinkles.Module.Engine
{
    internal class SprinklesDecorator
    {
        private readonly IList<ITaskArgumentTypeConverter> _typeConverters;
        private readonly ICakeArguments _arguments;
        private readonly ICakeConfiguration _configuration;
        private readonly ICakeEnvironment _environment;
        private readonly SprinklesValidator _validator;
        private readonly SprinklesArgumentsProvider _argumentsProvider;
        public SprinklesDecorator(
            IEnumerable<ITaskArgumentTypeConverter> typeConverters, 
            ICakeArguments arguments, 
            ICakeConfiguration configuration, 
            ICakeEnvironment environment,
            SprinklesValidator validator,
            SprinklesArgumentsProvider argumentsProvider)
        {
            _typeConverters = typeConverters.ToList();
            _arguments = arguments;
            _configuration = configuration;
            _environment = environment;
            _validator = validator;
            _argumentsProvider = argumentsProvider;
        }

        public TTask Decorate<TTask>(TTask frostingTask)
            where TTask : IFrostingTask 
        {
            var exceptions = new List<SprinklesException>();

            var argumentsOfTask = _argumentsProvider.GetAllArguments(frostingTask, exceptions);
            ApplySprinkles(argumentsOfTask, exceptions);

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return frostingTask;
        }

        private void ApplySprinkles(IList<(PropertyInfo property, object? parent)> argumentsOfTask, IList<SprinklesException> exceptions)
        {
            var propertiesThatCanBeAllowedNames =
                argumentsOfTask
                    .Where(x => !SprinklesDecorations.IsExternalTaskArguments(x.property))
                    .ToList();
            
            var propertyNames = 
                propertiesThatCanBeAllowedNames
                    .Select(x => SprinklesDecorations.GetArgumentName(x.property))
                    .ToList();

            foreach (var record in argumentsOfTask)
            {
                var property = record.property;
                var name = SprinklesDecorations.GetArgumentName(property);
                var namespaceClassQualifiedPropertyName = SprinklesDecorations.GetNamespaceClassQualifiedPropertyName(property);
                var isEnumeration = SprinklesDecorations.IsEnumeration(property.PropertyType);
                var enumeratedType = SprinklesDecorations.GetEnumeratedType(property.PropertyType)!;
                if (exceptions.Any(x => x.NamespaceClassQualifiedPropertyName == namespaceClassQualifiedPropertyName))
                {
                    continue;
                }

                try
                {
                    SprinklesValidator.ValidateDuplicateBuildClassProperty(property, propertyNames);
                    _validator.ValidateBuildClassProperty(property);

                    object? args;

                    if (isEnumeration)
                    {
                        args = GetArguments(property);
                    }
                    else
                    {
                        args = GetArgument(property);
                    }

                    if (SprinklesDecorations.IsRequired(property))
                    {
                        if (args == null || args is ICollection { Count: 0 })
                        {
                            var message = SprinklesValidator.Message_ArgumentWasNotSet;
                            throw new SprinklesException(property, message);
                        }
                    }

                    SetDeepValue(argumentsOfTask, property, args);
                }
                catch (SprinklesException exception)
                {
                    if (isEnumeration)
                    {
                        var defaultValue = GetEnumerationResult(Cast(new List<object>(), property), property, enumeratedType);
                        SetDeepValue(argumentsOfTask, property, defaultValue);
                    }
                    exceptions.Add(exception);
                }
            }
        }

        private object? GetArgument(PropertyInfo property)
        {
            var validators = SprinklesDecorations.GetArgumentValidations(property);
            try
            {
                var taskArgument = new TaskArgument(_arguments, _configuration, _environment, property);

                var value = taskArgument.GetValue();
                if (!SprinklesDecorations.IsFlag(property) && !string.IsNullOrWhiteSpace(value))
                {
                    foreach(var validator in validators)
                    {
                        validator.Validate(property, value);
                    }
                }

                if (TryConvertCustom(taskArgument, out var result))
                {
                    return result;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return Parse(property, value, property.PropertyType);
                }

                return null;
            }
            catch (TargetInvocationException e)
            {
                throw new SprinklesException(property, e.InnerException!.Message);
            }

        }

        private object? GetArguments(PropertyInfo property)
        {
            var enumeratedType = SprinklesDecorations.GetEnumeratedType(property.PropertyType)!;
            var delimiter = SprinklesDecorations.GetArgumentEnumerationDelimiterName(property);
            var hasDelimiter = !string.IsNullOrWhiteSpace(delimiter);
            var validators = SprinklesDecorations.GetArgumentValidations(property);

            try
            {
                var taskArgument = new TaskArgument(_arguments, _configuration, _environment, property);

                var values = taskArgument.GetValues();
                //// At this point it's a collection of strings
                SprinklesValidator.ValidateEnumerableRuntimeArguments(values, property);

                var list = (IList<string>)GetMethodEnumerableToList(typeof(string)).Invoke(null, new object[]{ values! })!;

                if (list.Count == 1 && hasDelimiter)
                {
                    list = list.First().Split(delimiter).ToList();
                }
              
                if (list.Any())
                {
                    foreach (var validator in validators)
                    {
                        validator.Validate(property, list);
                    }
                }

                if (TryConvertCustom(taskArgument, out var result))
                {
                    return result;
                }

                var typedValues = Cast(list, property);

                return GetEnumerationResult(typedValues, property, enumeratedType);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is SprinklesException)
                {
                    throw e.InnerException;
                }

                throw new SprinklesException(property, e.InnerException!.Message);
            } 
        }

        private bool TryConvertCustom(TaskArgument argument, out object? result) {
            var converters = _typeConverters.Where(x => x.ConversionType == argument.Type).Select(x => (TypeConverter)x).ToList();
            TypeConverter preferredConverter;
            if (!converters.Any() || converters.Count > 1)
            {
                preferredConverter = 
                    SprinklesDecorations.GetArgumentConverter(argument.Property, _typeConverters) 
                        as TypeConverter 
                    ?? TypeDescriptor.GetConverter(argument.Type);
            } 
            else
            {
                preferredConverter = converters.First();
            }

            if (preferredConverter == null
                || !preferredConverter.CanConvertFrom(typeof(TaskArgument)))
            {
                result = null;
                return false;
            }

            result = preferredConverter.ConvertFrom(argument);
            return result != null;
        }

        private object Cast(object values, PropertyInfo property)
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


        /// <summary>
        /// Converts a string value into the type specified.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The string value.</param>
        /// <param name="type">The type to convert it to.</param>
        /// <returns>An object of the base type specified.</returns>
        /// <exception cref="SprinklesException">If the type cannot be cast.</exception>
        private object? Parse(PropertyInfo property, string? value, Type type)
        {
            var exceptionMessage = SprinklesValidator.Message_ArgumentWasNotAValidValueForType;

            if (value == null)
            {
                return null;
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                return converter.ConvertFromInvariantString(value)!;
            }
            catch(Exception e) 
            {
                if (e is SprinklesCaptureException)
                {
                    throw;
                }

                var constructor = type.GetConstructors()
                    .FirstOrDefault(x => x.GetParameters().Length == 1 && !x.ContainsGenericParameters);
                if (constructor != null)
                {
                    try
                    {
                        return constructor.Invoke(new object[] { value });
                    }
                    catch
                    {

                    }
                }
            }

            if (value == string.Empty)
            {
                return null;
            }

            throw new SprinklesException(
                   property,
                   exceptionMessage,
                   $"Argument: {value}",
                   $"Type: {type.Name}",
                   SprinklesValidator.Message_BeSureToAddTypeConverter);
        }

        private static void SetDeepValue(IList<(PropertyInfo property, object? parent)> propertiesOfObjectsToUpdate, PropertyInfo property, object? value)
        {
            var record = propertiesOfObjectsToUpdate.First(x => x.property == property);
            property.SetValue(record.parent, value);
        }

        private static object GetEnumerationResult(object value, PropertyInfo property, Type enumeratedType)
        {
            if (SprinklesDecorations.IsList(property.PropertyType))
            {
                return ToImmutableList(value, enumeratedType);
            }

            return ToImmutableHashSet(value, enumeratedType);
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
