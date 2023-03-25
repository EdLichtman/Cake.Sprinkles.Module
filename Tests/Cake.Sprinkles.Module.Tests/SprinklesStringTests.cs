using Cake.Sprinkles.Module.Tests.Models.StringTasks;
using Cake.Sprinkles.Module.Validation;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesStringTests : SprinklesTestBase
    {
        private IList<string> RequiredArguments = new List<string>
        {
                nameof(StringRequiredTask.Single),
                nameof(StringRequiredTask.List),
                nameof(StringRequiredTask.HashSet)
        };

        private IList<string> OptionalArguments = new List<string>
        {
                nameof(StringOptionalTask.Single),
                nameof(StringOptionalTask.List),
                nameof(StringOptionalTask.HashSet)
        };

        private IList<string> ArgumentValues = new List<string>
        {
            "foo",
            "bar",
            "foobar"
        };

        [Test]
        public void CanRequireMultipleArguments()
        {
            var result = GetCakeHost<StringRequiredTask>().Run(FormatCustomArguments(nameof(StringRequiredTask)));

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
            var result = GetCakeHost<StringRequiredTask>().Run(FormatCustomArguments(nameof(StringRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringRequiredTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<StringRequiredTask>().Run(FormatCustomArguments(nameof(StringRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<StringRequiredTask>().Run(FormatCustomArguments(nameof(StringRequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringRequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<StringOptionalTask>().Run(FormatCustomArguments(nameof(StringOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringOptionalTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<StringOptionalTask>().Run(FormatCustomArguments(nameof(StringOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<StringOptionalTask>().Run(FormatCustomArguments(nameof(StringOptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringOptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanInstantiateNullOptionalList()
        {
            var result = GetCakeHost<StringOptionalTask>().Run(FormatCustomArguments(nameof(StringOptionalTask)));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringOptionalTask>()?.List, Is.Not.Null);
        }

        [Test]
        public void CanInstantiateNullOptionalHashSet()
        {
            var result = GetCakeHost<StringOptionalTask>().Run(FormatCustomArguments(nameof(StringOptionalTask)));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<StringOptionalTask>()?.HashSet, Is.Not.Null);
        }
    }
}
