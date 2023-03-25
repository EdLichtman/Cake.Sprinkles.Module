using Cake.Sprinkles.Module.Tests.Models.DecimalTasks;
using Cake.Sprinkles.Module.Tests.Models.GuidTasks;
using Cake.Sprinkles.Module.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesGuidTests : SprinklesTestBase
    {
        private IList<string> RequiredArguments = new List<string>
        {
                nameof(GuidRequiredTask.Single),
                nameof(GuidRequiredTask.List),
                nameof(GuidRequiredTask.HashSet)
        };

        private IList<string> OptionalArguments = new List<string>
        {
                nameof(GuidOptionalTask.Single),
                nameof(GuidOptionalTask.List),
                nameof(GuidOptionalTask.HashSet)
        };

        private IList<Guid> ArgumentValues = new List<Guid>
        {
            new Guid("56182CA7-ADBF-4D17-B3B2-4343E7ABFB24"),
            new Guid("CB2961EA-72A3-4145-BDDE-72B90076EFC6"),
            new Guid("2A451C27-B6DC-4449-83F7-90771FFCEE23")
        };

        [Test]
        public void ThrowsErrorOnRequiredPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(RequiredArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<GuidRequiredTask>().Run(FormatCustomArguments(nameof(GuidRequiredTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in RequiredArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Guid"));
                }
            });
        }

        [Test]
        public void ThrowsErrorOnOptionalPropertiesIfCastingIsInvalid()
        {
            var result = RunCakeHost<GuidRequiredTask>(OptionalArguments.Select(x => (x, "foo")));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in OptionalArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Guid"));
                }
            });
        }

        [Test]
        public void CanRequireMultipleArguments()
        {
            var result = RunCakeHost<GuidRequiredTask>();

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in RequiredArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotSet));
                }
            });
        }

        [Test]
        public void CanDecorateRequiredSingleWithLastValue()
        {
            var result = RunCakeHost<GuidRequiredTask>(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<GuidRequiredTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var result = RunCakeHost<GuidRequiredTask>(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<GuidRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var result = RunCakeHost<GuidRequiredTask>(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<GuidRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var result = RunCakeHost<GuidOptionalTask>(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<GuidOptionalTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var result = RunCakeHost<GuidOptionalTask>(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<GuidOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var result = RunCakeHost<GuidOptionalTask>(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<GuidOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }
    }
}
