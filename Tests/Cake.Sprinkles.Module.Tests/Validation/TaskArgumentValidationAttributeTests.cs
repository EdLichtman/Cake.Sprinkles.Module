using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.Validation;
using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Validation
{
    [TestFixture]
    internal class TaskArgumentValidationAttributeTests : SprinklesTestBase
    {
        [Test] 
        public void ThrowsSprinklesCaptureExceptionIfExceptionIsThrown()
        {
            RunCakeHost<ValidationTask>((nameof(ValidationTask.ThrowsExceptionInValidator), "foo"));

            var exception = SprinklesTestContextProvider.ThrownException as SprinklesCaptureException;
            Assert.That(exception?.InnerMessage, Is.EqualTo(TaskArgumentValidationAttribute.Message_IsArgumentValidShouldNotThrow));
        }

        [Test]
        public void ThrowsSprinklesCaptureExceptionWithInitiatingType()
        {
            RunCakeHost<ValidationTask>((nameof(ValidationTask.ThrowsExceptionInValidator), "foo"));

            var exception = SprinklesTestContextProvider.ThrownException as SprinklesCaptureException;
            Assert.That(exception?.Initiator, Is.EqualTo(typeof(ThrowsErrorValidationAttribute)));
        }

        [Test]
        public void DoesNotValidateEmptyStrings()
        {
            RunCakeHost<ValidationTask>((nameof(ValidationTask.ThrowsExceptionInValidator), string.Empty));

            Assert.That(SprinklesTestContextProvider.ThrownException, Is.Null);
            Assert.That(GetContext<ValidationTask>(), Is.Not.Null);
        }

        [TestCase(1, false)]
        [TestCase(3, true)]
        [TestCase(6, false)]
        public void CanHaveMultipleTaskArgumentValidators(int input, bool expectsSuccess)
        {
            RunCakeHost<ValidationTask>((nameof(ValidationTask.AllowsMoreThanOneValidator), input.ToString()));

            if (expectsSuccess)
            {
                var exception = SprinklesTestContextProvider.ThrownException;
                Assert.That(exception, Is.Null);
                Assert.That(GetContext<ValidationTask>()?.AllowsMoreThanOneValidator, Is.EqualTo(input));
            } 
            else
            {
                var exception = GetSprinklesExceptionForProperty<ValidationTask>(nameof(ValidationTask.AllowsMoreThanOneValidator));
                Assert.That(exception, Is.Not.Null);
            }
        }

        [Test]
        public void TaskArgumentValidationErrorThrowsConfiguredErrorMessage()
        {
            var input = 6;
            RunCakeHost<ValidationTask>((nameof(ValidationTask.ThrowsExceptionIfMoreThanFive), input.ToString()));

            var exception = GetSprinklesExceptionForProperty<ValidationTask>(nameof(ValidationTask.ThrowsExceptionIfMoreThanFive));
            Assert.That(exception?.InnerMessage, Is.EqualTo(ValidateIntegerLessThanFiveAttribute.ErrorMessageConstant));
        }

        [Test]
        public void TaskArgumentValidationErrorForSingleThrowsInvalidArgument()
        {
            var input = 6;
            RunCakeHost<ValidationTask>((nameof(ValidationTask.ThrowsExceptionIfMoreThanFive), input.ToString()));

            var exception = GetSprinklesExceptionForProperty<ValidationTask>(nameof(ValidationTask.ThrowsExceptionIfMoreThanFive));
            Assert.That(exception?.AdditionalInformation?.Count(), Is.EqualTo(1));
            Assert.That(exception.AdditionalInformation.First(), Is.EqualTo($"Invalid Argument: {input}"));
        }

        [Test]
        public void TaskArgumentValidationErrorForEnumerableThrowsAllInvalidArguments()
        {
            var input1 = 6;
            var input2 = 7;
            RunCakeHost<ValidationTask>(
                (nameof(ValidationTask.EnumerableValidation), input1.ToString()),
                (nameof(ValidationTask.EnumerableValidation), input2.ToString())                );

            var exception = GetSprinklesExceptionForProperty<ValidationTask>(nameof(ValidationTask.EnumerableValidation));
            Assert.That(exception?.AdditionalInformation?.Count(), Is.EqualTo(2));
            Assert.That(exception.AdditionalInformation[0], Is.EqualTo($"Invalid Argument: {input1}"));
            Assert.That(exception.AdditionalInformation[1], Is.EqualTo($"Invalid Argument: {input2}"));
        }
    }
}
