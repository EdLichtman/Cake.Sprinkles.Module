using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Tests.Models.InvalidTasks;
using Cake.Sprinkles.Module.Validation;

namespace Cake.Sprinkles.Module.Tests
{
    internal class SprinklesInvalidArgumentsTests : SprinklesTestBase
    {
        [Test]
        public void ThrowsExceptionIfTaskContainsEnumerableThatIsNotImmutableListOrHashSet()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidEnumerableProperty);

            RunCakeHost<InvalidTask>((knownInvalidTaskNameProperty, "foo"));

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_EnumerableMustImplementImmutableList));
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagButNoBoolean()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidFlagProperty);

            RunCakeHost<InvalidTask>((knownInvalidTaskNameProperty, "foo"));

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagMustBeBoolean));
        }

        [Test]
        public void ThrowsExceptionIfTaskArgumentFlagIsRequired()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyRequiredFlagProperty);

            RunCakeHost<InvalidTask>((knownInvalidTaskNameProperty, "foo"));

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagCannotBeRequired));
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagThatIsEnumerable()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyEnumerableFlagProperty);

            RunCakeHost<InvalidTask>((knownInvalidTaskNameProperty, "foo"));

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagMustNotBeEnumerable));
        }

        [Test]
        public void AddsDescriptionToRequiredArgument()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyRequiredProperty);

            RunCakeHost<InvalidTask>();

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.TaskArgumentDescription, Is.EqualTo(InvalidTask.MyRequiredPropertyDescription));
            Assert.That(exception.Message, Does.Contain($"Description: {InvalidTask.MyRequiredPropertyDescription}"));
        }

        [Test]
        public void ThrowsExceptionIfInputHasDelimiterAndMoreThanOneArgumentPassedIn()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyDelimiterProperty);

            RunCakeHost<InvalidTask>(
                    (knownInvalidTaskNameProperty, "foo"),
                    (knownInvalidTaskNameProperty, "bar")                    );

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_EnumerableDelimiterCannotHaveMoreThanOneArgument));
        }

        [Test]
        public void ThrowsExceptionIfDelimiterIsSetOnArgumentWithNoEnumeration()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidDelimiterProperty);

            RunCakeHost<InvalidTask>(
                    (knownInvalidTaskNameProperty, "foo"),
                    (knownInvalidTaskNameProperty, "bar"));

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_EnumerableDelimiterMustImplementEnumerable));
        }

        [Test]
        public void ErrorThrownBySprinklesWithNoDescriptionHasNoDescriptionInMessage()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidEnumerableProperty);

            RunCakeHost<InvalidTask>();

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            var exception = exceptions.FirstOrDefault(ex => ex.TaskArgumentName == knownInvalidTaskNameProperty);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Not.Contain("Description:"));
        }

        [Test]
        public void ThrowsErrorsIfArgumentAppearsOnMoreThanOneProperty()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyDuplicateArgumentName);

            RunCakeHost<InvalidTask>();

            var exceptions = GetSprinklesExceptions()?.Where(x => x.TaskArgumentName == knownInvalidTaskNameProperty)?.ToList();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);

            Assert.That(exceptions.Count, Is.EqualTo(3));
            
            var firstDuplicateArgument = FilterSprinklesExceptionForProperty<InvalidTask>(exceptions, nameof(InvalidTask.ParentArgument));
            Assert.That(firstDuplicateArgument?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentCannotAppearMoreThanOneTimeOnTask));

            var secondDuplicateArgument = FilterSprinklesExceptionForProperty<ChildInvalidArguments>(exceptions, nameof(ChildInvalidArguments.ChildArgument));
            Assert.That(secondDuplicateArgument?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentCannotAppearMoreThanOneTimeOnTask));

            var thirdDuplicateArgument = FilterSprinklesExceptionForProperty<ChildInvalidArgumentsOneDeep>(exceptions, nameof(ChildInvalidArgumentsOneDeep.ChildArgument));
            Assert.That(thirdDuplicateArgument?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentCannotAppearMoreThanOneTimeOnTask));
        }

        [Test(
            Description = "The point of this test is so that we don't get an AggregateException as one of the InnerExceptions of an AggregateException. " +
            "We flatten all the exceptions into a standard array.")]
        public void ThrowsAggregateExceptionByAggregatingAllErrorsWhenUsingExternalArguments()
        {
            RunCakeHost<InvalidTask>();
            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);
            Assert.IsTrue(exceptions.Any(exception => exception.TaskArgumentName == nameof(ChildInvalidArguments.RequiredChildArgument)));
            Assert.IsTrue(exceptions.Any(exception => exception.TaskArgumentName == nameof(ChildInvalidArguments.OtherRequiredChildArgument)));
        }

        [Test]
        public void ThrowsExceptionIfExternalArgumentsHasAnyOtherTaskArgumentAttribute()
        {
            RunCakeHost<InvalidTask>();
            var exception = GetSprinklesExceptionForProperty<InvalidTask>(nameof(InvalidTask.ParentInvalidChildArguments));
            
            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_TaskArgumentsCannotHaveDescriptors));
        }

        [Test]
        public void ExternalTaskArgumentsClassWithoutParameterlessConstructorThrowsException()
        {
            RunCakeHost<InvalidTask>();
            var exception = GetSprinklesExceptionForProperty<InvalidTask>(nameof(InvalidTask.ParameterConstructorClassChildArguments));

            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_TaskArgumentsMustHaveParameterlessConstructor));
        }

        [Test]
        public void ExternalTaskArgumentsStructThrowsException()
        {
            RunCakeHost<InvalidTask>();
            var exception = GetSprinklesExceptionForProperty<InvalidTask>(nameof(InvalidTask.ParameterConstructorStructChildArguments));

            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_StructNotSupported));
        }

        [Test]
        public void ValidationNotAllowedOnFlags()
        {
            RunCakeHost<InvalidTask>();
            var exception = GetSprinklesExceptionForProperty<InvalidTask>(nameof(InvalidTask.MyValidationFlagProperty));

            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagCannotBeValidated));
        }
    }
}
