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
            _host.Run(
                FormatCustomArguments(
                    (key: nameof(InvalidTask.MyInvalidEnumerableProperty), value: "foo"),
                    (key: nameof(InvalidTask.MyRequiredProperty), value: "bar")
                ));

            var exception = ThrownException?.InnerExceptions.FirstOrDefault() as SprinklesException;
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo($"An enumerable Argument must implement {nameof(ImmutableList)} or {nameof(ImmutableHashSet)}"));
            Assert.That(exception.TaskArgumentName, Is.EqualTo(nameof(InvalidTask.MyInvalidEnumerableProperty)));
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagButNoBoolean()
        {
            _host.Run(
                           FormatCustomArguments(
                               (key: nameof(InvalidTask.MyInvalidFlagProperty), value: "foo"),
                               (key: nameof(InvalidTask.MyRequiredProperty), value: "bar")
                           ));

            var exception = ThrownException?.InnerExceptions.FirstOrDefault() as SprinklesException;
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.InnerMessage, Is.EqualTo("An argument that accepts a flag must be a boolean."));
            Assert.That(exception.TaskArgumentName, Is.EqualTo(nameof(InvalidTask.MyInvalidFlagProperty)));
        }

        [Test]
        public void ThrowsExceptionIfTaskArgumentFlagIsRequired()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagThatIsEnumerable()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddsDescriptionToRequiredArgument()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        public void ThrowsExceptionIfInputHasDelimiterAndMoreThanOneArgumentPassedIn()
        {
            throw new NotImplementedException(); 
        }

        [Test]
        public void ThrowsExceptionIfDelimiterIsSetOnArgumentWithNoEnumeration()
        {
            throw new NotImplementedException(); 
        }

        [Test]
        public void ErrorThrownBySprinklesWithNoDescriptionHasNoDescriptionInMessage()
        {
            throw new NotImplementedException();
        }
    }
}
