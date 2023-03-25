using Cake.Sprinkles.Module.Tests.Models.DecimalTasks;
using Cake.Sprinkles.Module.Validation;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesDecimalTests : SprinklesTestBase
    {
        private IList<string> RequiredArguments = new List<string>
        {
                nameof(DecimalRequiredTask.Single),
                nameof(DecimalRequiredTask.List),
                nameof(DecimalRequiredTask.HashSet)
        };

        private IList<string> OptionalArguments = new List<string>
        {
                nameof(DecimalOptionalTask.Single),
                nameof(DecimalOptionalTask.List),
                nameof(DecimalOptionalTask.HashSet)
        };

        private IList<decimal> ArgumentValues = new List<decimal>
        {
            1.1m,2.2m,3.3m
        };

        [Test]
        public void ThrowsErrorOnRequiredPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(RequiredArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<DecimalRequiredTask>().Run(FormatCustomArguments(nameof(DecimalRequiredTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in RequiredArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Decimal"));
                }
            });
        }

        [Test]
        public void ThrowsErrorOnOptionalPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(OptionalArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<DecimalOptionalTask>().Run(FormatCustomArguments(nameof(DecimalOptionalTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in OptionalArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Decimal"));
                }
            });
        }

        [Test]
        public void CanRequireMultipleArguments()
        {
            var result = GetCakeHost<DecimalRequiredTask>().Run(FormatCustomArguments(nameof(DecimalRequiredTask)));

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
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<DecimalRequiredTask>().Run(FormatCustomArguments(nameof(DecimalRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<DecimalRequiredTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<DecimalRequiredTask>().Run(FormatCustomArguments(nameof(DecimalRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<DecimalRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<DecimalRequiredTask>().Run(FormatCustomArguments(nameof(DecimalRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<DecimalRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<DecimalOptionalTask>().Run(FormatCustomArguments(nameof(DecimalOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<DecimalOptionalTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<DecimalOptionalTask>().Run(FormatCustomArguments(nameof(DecimalOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<DecimalOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<DecimalOptionalTask>().Run(FormatCustomArguments(nameof(DecimalOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<DecimalOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }
    }
}
