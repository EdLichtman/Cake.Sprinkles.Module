using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.BooleanTasks;
using Cake.Sprinkles.Module.Tests.Models.Int32Tasks;
using Cake.Sprinkles.Module.Validation;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesBooleanTaskTests : SprinklesTestBase
    {
        private IList<string> RequiredArguments = new List<string>
        {
                nameof(BooleanRequiredTask.Single),
                nameof(BooleanRequiredTask.List),
                nameof(BooleanRequiredTask.HashSet)
        };

        private IList<string> OptionalArguments = new List<string>
        {
                nameof(BooleanOptionalTask.Single),
                nameof(BooleanOptionalTask.List),
                nameof(BooleanOptionalTask.HashSet)
        };

        private IList<bool> ArgumentValues = new List<bool>
        {
            false, true
        };

        [Test]
        public void ThrowsErrorOnRequiredPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(RequiredArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<BooleanRequiredTask>().Run(FormatCustomArguments(nameof(BooleanRequiredTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in RequiredArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Boolean"));
                }
            });
        }

        [Test]
        public void ThrowsErrorOnOptionalPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(OptionalArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<BooleanOptionalTask>().Run(FormatCustomArguments(nameof(BooleanOptionalTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in OptionalArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Boolean"));
                }
            });
        }

        [Test]
        public void CanRequireMultipleArguments()
        {
            var result = GetCakeHost<BooleanRequiredTask>().Run(FormatCustomArguments(nameof(BooleanRequiredTask)));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.And.Not.Empty);
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
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<BooleanRequiredTask>().Run(FormatCustomArguments(nameof(BooleanRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<BooleanRequiredTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<BooleanRequiredTask>().Run(FormatCustomArguments(nameof(BooleanRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<BooleanRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<BooleanRequiredTask>().Run(FormatCustomArguments(nameof(BooleanRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<BooleanRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<BooleanOptionalTask>().Run(FormatCustomArguments(nameof(BooleanOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<BooleanOptionalTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<BooleanOptionalTask>().Run(FormatCustomArguments(nameof(BooleanOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<BooleanOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<BooleanOptionalTask>().Run(FormatCustomArguments(nameof(BooleanOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<BooleanOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }
    }
}