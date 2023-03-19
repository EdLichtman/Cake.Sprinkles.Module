using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models.Int32;
using Cake.Sprinkles.Module.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Sprinkles.Module.Tests.Models.InvalidTasks;
using System.Collections.Immutable;

namespace Cake.Sprinkles.Module.Tests
{
    internal class SprinklesInvalidArgumentsTests : SprinklesTestBase
    {

        private CakeHost _host = null!;
        private Int32Task? Context => (SprinklesTestContextProvider.Context as SprinklesTestContext<Int32Task>)?.Task;
        private AggregateException? ThrownException => SprinklesTestContextProvider.ThrownException as AggregateException;

        [SetUp]
        public void SetUp() 
        {
            _host = this.GetCakeHost<InvalidTask>();
        }
        [Test]
        public void ThrowsExceptionIfTaskContainsEnumerableThatIsNotImmutableListOrHashSet()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidEnumerableProperty);
            _host.Run(
                FormatCustomArguments(
                    (key: knownInvalidTaskNameProperty, value: "foo")
                ));

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex => 
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_EnumerableMustImplementImmutableList));
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagButNoBoolean()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidFlagProperty);
            _host.Run(
                FormatCustomArguments(
                    (key: knownInvalidTaskNameProperty, value: "foo")
                ));

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagMustBeBoolean));
        }

        [Test]
        public void ThrowsExceptionIfTaskArgumentFlagIsRequired()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyRequiredFlagProperty);
            _host.Run(
                FormatCustomArguments(
                    (key: knownInvalidTaskNameProperty, value: "foo")
                ));

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagCannotBeRequired));
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagThatIsEnumerable()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyEnumerableFlagProperty);
            _host.Run(
                FormatCustomArguments(
                    (key: knownInvalidTaskNameProperty, value: "foo")
                ));

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_FlagMustNotBeEnumerable));
        }

        [Test]
        public void AddsDescriptionToRequiredArgument()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyRequiredProperty);
            _host.Run(FormatCustomArguments());

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.TaskArgumentDescription, Is.EqualTo(InvalidTask.MyRequiredPropertyDescription));
            Assert.That(exception.Message, Does.Contain($"Description: {InvalidTask.MyRequiredPropertyDescription}"));
        }
        
        [Test]
        public void ThrowsExceptionIfInputHasDelimiterAndMoreThanOneArgumentPassedIn()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyDelimiterProperty);
            _host.Run(
                FormatCustomArguments(
                    (key: knownInvalidTaskNameProperty, value: "foo"),
                    (key: knownInvalidTaskNameProperty, value: "bar")
                ));

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_EnumerableDelimiterCannotHaveMoreThanOneArgument));
        }

        [Test]
        public void ThrowsExceptionIfDelimiterIsSetOnArgumentWithNoEnumeration()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidDelimiterProperty);
            _host.Run(
                FormatCustomArguments(
                    (key: knownInvalidTaskNameProperty, value: "foo")
                ));

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo(SprinklesValidator.Message_EnumerableDelimiterMustImplementEnumerable));
        }

        [Test]
        public void ErrorThrownBySprinklesWithNoDescriptionHasNoDescriptionInMessage()
        {
            var knownInvalidTaskNameProperty = nameof(InvalidTask.MyInvalidEnumerableProperty);
            _host.Run(FormatCustomArguments());

            var exception = ThrownException?
                    .InnerExceptions
                    .FirstOrDefault(ex =>
                        (ex as SprinklesException)?.TaskArgumentName == knownInvalidTaskNameProperty
                    ) as SprinklesException;

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Not.Contain("Description:"));
        }
    }
}
