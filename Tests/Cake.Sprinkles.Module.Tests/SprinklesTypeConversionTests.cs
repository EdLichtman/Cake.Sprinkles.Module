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
        [Test]
        public void TypeConversionConvertsStringIntoType()
        {
            const string input1 = nameof(input1);
            const string input2 = nameof(input2);

            GetCakeHost<TypeConverterTask>()
                .RegisterTypeConverter<TypeWithUsageConverter>()
                .RegisterTypeConverter<TypeWithoutUsageConverter>()
                    .Run(FormatCustomArguments(nameof(TypeConverterTask),
                    (nameof(TypeConverterTask.ConversionTypeWithUsage), input1),
                    (nameof(TypeConverterTask.OtherConversionType), input2)));

            var context = GetContext<TypeConverterTask>();
            Assert.That(context?.ConversionTypeWithUsage?.InternalProperty, Is.EqualTo(input1));
            Assert.That(context?.OtherConversionType?.InternalProperty, Is.EqualTo(input2));
        }

        [Test]
        public void TypeConversionExceptionContainsSuggestionThatTypeConverterDoesNotExist()
        {
            const string input1 = nameof(input1);
            const string input2 = nameof(input2);

            RunCakeHost<TypeConverterTask>(
                (nameof(TypeConverterTask.ConversionTypeWithUsage), input1),
                (nameof(TypeConverterTask.OtherConversionType), input2));

            var context = GetContext<TypeConverterTask>();
            Assert.That(context?.ConversionTypeWithUsage, Is.Null);
            Assert.That(context?.OtherConversionType, Is.Null);

            var exception = GetSprinklesExceptionForProperty<TypeConverterTask>(nameof(TypeConverterTask.ConversionTypeWithUsage));
            Assert.That(exception?.AdditionalInformation, Does.Contain(SprinklesValidator.Message_BeSureToAddTypeConverter));
        }

        [Test]
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

        [Test]
        public void TypeConversionCanBeUsedForLists()
        {
            const string input1 = nameof(input1);

            GetCakeHost<TypeConverterTask>()
                .RegisterTypeConverter<TypeWithUsageListConverter>()
                .Run(FormatCustomArguments(
                (nameof(TypeConverterTask.ConversionTypeWithListUsage), input1)));

            var context = GetContext<TypeConverterTask>();
            Assert.That(context!.ConversionTypeWithListUsage, Is.Not.Null.And.Not.Empty);
            Assert.That(context!.ConversionTypeWithListUsage[0].InternalProperty, Is.EqualTo(input1));
        }

        [Test]
        public void TypeConversionThrowsExceptionIfMoreThanOneExistWithNoSprinklesDecoration()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TypeConversionAllowsMoreThanOneConverterForType()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TypeConverterAttributeWithInvalidTypeConverterTypeThrowsException()
        {
            throw new NotImplementedException();
        }
    }
}
