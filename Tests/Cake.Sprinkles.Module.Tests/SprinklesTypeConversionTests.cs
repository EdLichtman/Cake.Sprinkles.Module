using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.TypeConversion;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesTypeConversionTests : SprinklesTestBase
    {
        [Test(Description = "TypeConverter works")]
        public void TypeConversionConvertsStringIntoType()
        {
            const string input1 = nameof(input1);
            const string input2 = nameof(input2);

            GetCakeHost<TypeConverterTask>()
                .RegisterTypeConverter<TypeWithUsageConverter>()
                .RegisterTypeConverter<TypeWithoutUsageConverter>()
                .RegisterTypeConverter<TypeWithUsageListConverter>()
                    .Run(FormatCustomArguments(nameof(TypeConverterTask),
                    (nameof(TypeConverterTask.ConversionTypeWithUsage), input1),
                    (nameof(TypeConverterTask.OtherConversionType), input2)));

            var context = GetContext<TypeConverterTask>();
            Assert.That(context?.ConversionTypeWithUsage?.InternalProperty, Is.EqualTo(input1));
            Assert.That(context?.OtherConversionType?.InternalProperty, Is.EqualTo(input2));
        }

        [Test(Description = "TypeConverter not existing will cause reminder text to be told.")]
        public void TypeConversionExceptionContainsSuggestionThatTypeConverterDoesNotExist()
        {
            const string input1 = nameof(input1);
            const string input2 = nameof(input2);

            RunCakeHost<TypeConverterTask>(
                (nameof(TypeConverterTask.ConversionTypeWithUsage), input1),
                (nameof(TypeConverterTask.OtherConversionType), input2),
                (nameof(TypeConverterTask.ConversionTypeWithListUsage), input1));

            var context = GetContext<TypeConverterTask>();
            Assert.That(context?.ConversionTypeWithUsage, Is.Null);
            Assert.That(context?.OtherConversionType, Is.Null);

            var exception = GetSprinklesExceptionForProperty<TypeConverterTask>(nameof(TypeConverterTask.ConversionTypeWithUsage));
            Assert.That(exception?.AdditionalInformation, Does.Contain(SprinklesValidator.Message_BeSureToAddTypeConverter));

            exception = GetSprinklesExceptionForProperty<TypeConverterTask>(nameof(TypeConverterTask.OtherConversionType));
            Assert.That(exception?.AdditionalInformation, Does.Contain(SprinklesValidator.Message_BeSureToAddTypeConverter));


            exception = GetSprinklesExceptionForProperty<TypeConverterTask>(nameof(TypeConverterTask.ConversionTypeWithListUsage));
            Assert.That(exception?.AdditionalInformation, Does.Contain(SprinklesValidator.Message_BeSureToAddTypeConverter));
        }

        [Test(Description = "If an error is thrown during type conversion, it's a pretty fatal error.")]
        public void TypeConversionExceptionThrowsCaptureException()
        {
            const string input1 = nameof(input1);

            GetCakeHost<TypeConverterTask>()
                .RegisterTypeConverter<TypeConversionThatErrorsTypeConverter>()
                    .Run(FormatCustomArguments(nameof(TypeConverterTask),
                    (nameof(TypeConverterTask.TypeConversionThatErrors), input1)));

            var exception = SprinklesTestContextProvider.ThrownException as SprinklesCaptureException;
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception?.InnerException?.Message, Is.EqualTo(TypeConversionThatErrorsTypeConverter.ExpectedInternalException));
        }

        [Test(Description = "Lists can be conversion targets.")]
        public void TypeConversionCanBeUsedForLists()
        {
            const string input1 = nameof(input1);

            GetCakeHost<TypeConverterTask>()
                .RegisterTypeConverter<TypeWithUsageListConverter>()
                .Run(FormatCustomArguments(
                    nameof(TypeConverterTask),
                (nameof(TypeConverterTask.ConversionTypeWithListUsage), input1)));

            var context = GetContext<TypeConverterTask>();
            Assert.That(context!.ConversionTypeWithListUsage, Is.Not.Null.And.Not.Empty);
            Assert.That(context!.ConversionTypeWithListUsage[0].InternalProperty, Is.EqualTo(input1));
        }

        [Test(Description = "If more than one TypeConverter exist for the same type, a TaskArgument MUST have a TypeConverter attribute.")]
        public void TypeConversionThrowsExceptionIfMoreThanOneExistWithNoSprinklesDecoration()
        {
            GetCakeHost<TypeConverterInvalidTask>()
                .RegisterTypeConverter<TypeWithUsageConverter>()
                .RegisterTypeConverter<TypeWithUsageOtherConverter>()
                .Run(FormatCustomArguments(nameof(TypeConverterInvalidTask)));
            var context = GetContext<TypeConverterInvalidTask>();
            //Proves that it doesn't populate, even though we're not passing in an argument for InvalidTypeConverter
            Assert.That(context?.TypeThatDoesNotNeedConversion, Is.Null);

            var exception = GetSprinklesExceptionForProperty<TypeConverterInvalidTask>(nameof(TypeConverterInvalidTask.ConversionTypeWithUsage));
            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentConverterMultipleMustHaveAnnotation));
        }

        [Test(Description = "More than one TypeConverter can exist for the same type.")]
        public void TypeConversionAllowsMoreThanOneConverterForType()
        {
            var input = "input value";
            GetCakeHost<TypeConverterTask>()
               .RegisterTypeConverter<TypeWithUsageConverter>()
               .RegisterTypeConverter<TypeWithUsageOtherConverter>()
               .RegisterTypeConverter<TypeWithUsageListConverter>()
               .Run(FormatCustomArguments(
                   nameof(TypeConverterTask),
                   (nameof(TypeConverterTask.ConversionTypeWithUsage), input),
                   (nameof(TypeConverterTask.ConversionTypeWithUsageOther), input)
               ));

            var context = GetContext<TypeConverterTask>();

            Assert.That(context?.ConversionTypeWithUsage, Is.Not.Null);
            Assert.That(context?.ConversionTypeWithUsageOther, Is.Not.Null);
            Assert.That(context?.ConversionTypeWithUsage.InternalProperty, Is.EqualTo(input));
            Assert.That(context?.ConversionTypeWithUsageOther.InternalProperty, Is.EqualTo(TypeWithUsageOtherConverter.PrependedText + input));
            Assert.That(context?.ConversionTypeWithUsage.InternalProperty, Is.Not.EqualTo(context?.ConversionTypeWithUsageOther.InternalProperty));
        }

        [Test(Description = "A TypeConverter Attribute with an invalid type in the constructor will throw an understandable exception")]
        public void TypeConverterAttributeWithTypeThatDoesNotImplementTaskArgumentConverterThrowsException()
        {
            GetCakeHost<TypeConverterInvalidTask>()
                .RegisterTypeConverter<TypeWithUsageConverter>()
                .Run(FormatCustomArguments(nameof(TypeConverterInvalidTask)));
            var context = GetContext<TypeConverterInvalidTask>();
            //Proves that it doesn't populate, even though we're not passing in an argument for InvalidTypeConverter
            Assert.That(context?.TypeThatDoesNotNeedConversion, Is.Null);

            var exception = GetSprinklesExceptionForProperty<TypeConverterInvalidTask>(nameof(TypeConverterInvalidTask.TypeWithInvalidConverter));
            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentConverterNotValid));
        }

        [Test(Description = "A TypeConverter can be applied to a custom type in order to have it be automatically parsed for all instances.")]
        public void SystemComponentModelTypeConverterAttributeCanBeAppliedToCustomTypes()
        {
            var expectedValue = Guid.NewGuid().ToString();
            GetCakeHost<SystemComponentModelTypeConverterTask>()
                .Run(FormatCustomArguments(nameof(SystemComponentModelTypeConverterTask),
                (nameof(SystemComponentModelTypeConverterTask.MyCustomType), expectedValue)));
            
            var context = GetContext<SystemComponentModelTypeConverterTask>();
            
            //Proves that it doesn't populate, even though we're not passing in an argument for InvalidTypeConverter
            Assert.That(context?.MyCustomType?.MyCustomValue, Is.EqualTo(expectedValue));
        }

        [Test(Description = "A TypeConverter can be applied to a custom type in order to have it be automatically parsed for all instances.")]
        public void SystemComponentModelTypeConverterAttributeCanBeConvertedFromString()
        {
            var expectedValue = Guid.NewGuid().ToString();
            GetCakeHost<SystemComponentModelTypeConverterTask>()
                .Run(FormatCustomArguments(nameof(SystemComponentModelTypeConverterTask),
                (nameof(SystemComponentModelTypeConverterTask.MyOtherCustomType), expectedValue)));

            var context = GetContext<SystemComponentModelTypeConverterTask>();

            //Proves that it doesn't populate, even though we're not passing in an argument for InvalidTypeConverter
            Assert.That(context?.MyOtherCustomType?.MyCustomValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void RequiredTaskThrowsRequiredExceptionInsteadOfCastingException()
        {
            GetCakeHost<TypeConverterInvalidTask>()
                .Run(FormatCustomArguments(nameof(TypeConverterInvalidTask)));

            var exception = GetSprinklesExceptionForProperty<TypeConverterInvalidTask>(nameof(TypeConverterInvalidTask.RequiredType));
            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotSet));
        }
    }
}
