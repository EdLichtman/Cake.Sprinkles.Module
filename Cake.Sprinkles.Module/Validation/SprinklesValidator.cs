﻿using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.Annotations;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Cake.Sprinkles.Module.Validation.Exceptions;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Engine;

namespace Cake.Sprinkles.Module.Validation
{
    internal class SprinklesValidator
    {
        public const string Message_ArgumentWasNotSet = "Argument was not set.";
        public const string Message_ArgumentWasNotAValidValueForType = "Argument was not a valid value for type.";
        public const string Message_EnumerableMustImplementImmutableList = $"An enumerable Argument must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}";
        public const string Message_FlagMustNotBeEnumerable = "An argument that accepts a flag can only contain one value.";
        public const string Message_EnumerableDelimiterMustImplementEnumerable = $"An Argument with an enumerable delimiter must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}";
        public const string Message_FlagMustBeBoolean = "An argument that accepts a flag must be a boolean.";
        public const string Message_FlagCannotBeRequired = "An argument that accepts a flag cannot be a required argument.";
        public const string Message_FlagCannotBeValidated = $"An argument that accepts a flag cannot have an implementer of {nameof(TaskArgumentValidationAttribute)} annotation.";
        public const string Message_EnumerableDelimiterCannotHaveMoreThanOneArgument = "An argument that allows an enumeration delimiter must contain only one argument input. Any further inputs must be separated by a delimiter.";
        public const string Message_TaskArgumentsCannotHaveDescriptors = "For a property defined by a class containing task arguments, that property cannot have other task argument descriptors.";
        public const string Message_TaskArgumentsMustHaveParameterlessConstructor = "For a property defined by a class containing task arguments, that class must have a parameterless constructor.";
        public const string Message_ArgumentCannotAppearMoreThanOneTimeOnTask = "Argument cannot appear more than one time on a task, even across external task arguments.";
        public const string Message_StructNotSupported = "Argument cannot be a struct due to limitations for how value types are passed from stack frame to stack frame. If you wish to use a struct, first create a class to contain the information, and then you can utilize the struct on your own.";
        public const string Message_BeSureToAddTypeConverter = "Be sure to register any Custom TaskArgumentTypeConverters with the CakeHost.";
        public const string Message_ArgumentConverterNotValid = $"Argument converter is not valid. Be sure that the ArgumentConverterType implements {nameof(ITaskArgumentTypeConverter)}.";
        public const string Message_ArgumentConverterMultipleMustHaveAnnotation = $"There are multiple implementations of a type converter for your type. You must specify which to use with the {nameof(TaskArgumentConverterAttribute)}.";
        public const string Message_ArgumentConverterCouldNotConvertToCustomType = "Could not convert to custom type '{0}.{1}'. Be sure the TasArgumentTypeConverter handles all cases.";

        private readonly IList<ITaskArgumentTypeConverter> _typeConverters;
        public SprinklesValidator(IEnumerable<ITaskArgumentTypeConverter> typeConverters)
        {
            _typeConverters = typeConverters.ToList();
        }
        public void ValidateBuildClassProperty(PropertyInfo property)
        {
            var name = SprinklesDecorations.GetArgumentName(property);
            var isFlag = SprinklesDecorations.IsFlag(property);
            var isRequired = SprinklesDecorations.IsRequired(property);
            var hasDelimiter = !string.IsNullOrWhiteSpace(SprinklesDecorations.GetArgumentEnumerationDelimiterName(property));

            var enumeratedType = SprinklesDecorations.GetEnumeratedType(property.PropertyType);
            var isEnumeration = SprinklesDecorations.IsEnumeration(property.PropertyType);

            var isExternalArguments = SprinklesDecorations.IsExternalTaskArguments(property);

            var converters = _typeConverters.Where(x => x.ConversionType == property.PropertyType).ToList();
            var hasRegisteredConverter = converters.Any();
            var attributeConverterType = SprinklesDecorations.GetArgumentConverterType(property);
            
            if (attributeConverterType != null && !hasRegisteredConverter)
            {
                throw new SprinklesException(
                    property,
                    Message_BeSureToAddTypeConverter, 
                    $"Type={attributeConverterType.Name}");
            }
            if (converters.Any())
            {
                if (!SprinklesDecorations.IsArgumentConverterValid(property))
                {
                    throw new SprinklesException(
                        property,
                        Message_ArgumentConverterNotValid);
                }
            }

            if (converters.Count > 1)
            {
                var preferredConverter = SprinklesDecorations.GetArgumentConverter(property, _typeConverters);
                if (preferredConverter == null)
                {
                    throw new SprinklesException(
                        property,
                        Message_ArgumentConverterMultipleMustHaveAnnotation);
                }
            }

            if (!hasRegisteredConverter)
            {
                if (!isEnumeration)
                {
                    if (enumeratedType != null)
                    {
                        throw new SprinklesException(
                            property,
                            Message_EnumerableMustImplementImmutableList,
                            Message_BeSureToAddTypeConverter);
                    }

                    if (hasDelimiter)
                    {
                        throw new SprinklesException(
                            property,
                            Message_EnumerableDelimiterMustImplementEnumerable);
                    }
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

                var validators = SprinklesDecorations.GetArgumentValidations(property);
                if (validators.Any())
                {
                    throw new SprinklesException(
                        property,
                        Message_FlagCannotBeValidated);
                }
            }

            if (isExternalArguments)
            {
                var customAttributes = property.GetCustomAttributes();
                if (customAttributes.Any(
                    x => x.GetType().Name.StartsWith("TaskArgument")
                    && x.GetType() != typeof(TaskArgumentsAttribute)))
                {
                    throw new SprinklesException(
                        property,
                        Message_TaskArgumentsCannotHaveDescriptors);
                }

                var isStruct = property.PropertyType.IsValueType;
                var constructors = property.PropertyType.GetConstructors().ToList();
                var hasParameterlessConstructor = constructors.Any(x => x.GetParameters().Length == 0);

                if (isStruct)
                {
                    throw new SprinklesException(
                        property,
                        Message_StructNotSupported);
                }

                if (!hasParameterlessConstructor)
                {
                    throw new SprinklesException(
                        property,
                        Message_TaskArgumentsMustHaveParameterlessConstructor);
                }
            }
        }

        public static void ValidateDuplicateBuildClassProperty(PropertyInfo propertyInfo, IList<string> propertyNames)
        {
            var duplicatePropertyNames =
                propertyNames
                    .Distinct()
                    .Where(x => propertyNames.Count(y => y == x) > 1)
                    .ToList();

            var name = SprinklesDecorations.GetArgumentName(propertyInfo);
            if (name != null
                && duplicatePropertyNames.Contains(name))
            {
                throw new SprinklesException(
                    propertyInfo,
                    Message_ArgumentCannotAppearMoreThanOneTimeOnTask);
            }
        }

        public static void ValidateEnumerableRuntimeArguments(object? values, PropertyInfo property)
        {
            if (values is ICollection { Count: > 1 } && SprinklesDecorations.HasArgumentEnumerationDelimiter(property))
            {
                throw new SprinklesException(
                    property,
                    Message_EnumerableDelimiterCannotHaveMoreThanOneArgument,
                    $"Delimiter: {SprinklesDecorations.GetArgumentEnumerationDelimiterName(property)}");
            }
        }
    }
}
