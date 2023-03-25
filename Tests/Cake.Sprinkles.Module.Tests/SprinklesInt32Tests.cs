using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.Int32Tasks;
using Cake.Sprinkles.Module.Tests.Models.StringTasks;
using Cake.Sprinkles.Module.Validation;
using NuGet.Packaging;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesInt32Tests : SprinklesTestBase
    {
        private IList<string> RequiredArguments = new List<string>
        {
                nameof(Int32RequiredTask.Single),
                nameof(Int32RequiredTask.List),
                nameof(Int32RequiredTask.HashSet)
        };

        private IList<string> OptionalArguments = new List<string>
        {
                nameof(Int32OptionalTask.Single),
                nameof(Int32OptionalTask.List),
                nameof(Int32OptionalTask.HashSet)
        };

        private IList<string> ExternalArguments = new List<string>
        {
            nameof(Int32ExternalArgumentsTask.Single),
            nameof(Int32ExternalArguments.ExternalRequiredSingle),
            nameof(Int32ExternalArguments.ExternalRequiredList),
            nameof(Int32ExternalArguments.ExternalRequiredHashSet),
            nameof(Int32ExternalArguments.ExternalOptionalSingle),
            nameof(Int32ExternalArguments.ExternalOptionalList),
            nameof(Int32ExternalArguments.ExternalOptionalHashSet)
        };

        private IList<string> ExternalRequiredArguments = new List<string>
        {
            nameof(Int32ExternalArguments.ExternalRequiredSingle),
            nameof(Int32ExternalArguments.ExternalRequiredList),
            nameof(Int32ExternalArguments.ExternalRequiredHashSet),
        };

        private IList<int> ArgumentValues = new List<int>
        {
            1,2,3
        };

        [Test]
        public void ThrowsErrorOnRequiredPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(RequiredArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<Int32RequiredTask>().Run(FormatCustomArguments(nameof(Int32RequiredTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach(var expectedArgument in RequiredArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Int32"));
                }
            });
        }

        [Test]
        public void ThrowsErrorOnOptionalPropertiesIfCastingIsInvalid()
        {
            var properties = PrepareArguments(OptionalArguments.Select(x => (x, "foo")));
            var result = GetCakeHost<Int32OptionalTask>().Run(FormatCustomArguments(nameof(Int32OptionalTask), properties));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in OptionalArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
                    Assert.That(exception?.Message, Does.Contain("Type: Int32"));
                }
            });
        }

        [Test]
        public void CanRequireMultipleArguments()
        {
            var result = GetCakeHost<Int32RequiredTask>().Run(FormatCustomArguments(nameof(Int32RequiredTask)));

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
            var result = GetCakeHost<Int32RequiredTask>().Run(FormatCustomArguments(nameof(Int32RequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32RequiredTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32RequiredTask>().Run(FormatCustomArguments(nameof(Int32RequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32RequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var properties = PrepareArguments(RequiredArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32RequiredTask>().Run(FormatCustomArguments(nameof(Int32RequiredTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32RequiredTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32OptionalTask>().Run(FormatCustomArguments(nameof(Int32OptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32OptionalTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32OptionalTask>().Run(FormatCustomArguments(nameof(Int32OptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32OptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var properties = PrepareArguments(OptionalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32OptionalTask>().Run(FormatCustomArguments(nameof(Int32OptionalTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32OptionalTask>()?.List, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanInstantiateNullOptionalList()
        {
            var result = GetCakeHost<Int32OptionalTask>().Run(FormatCustomArguments(nameof(Int32OptionalTask)));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32OptionalTask>()?.List, Is.Not.Null);
        }

        [Test]
        public void CanInstantiateNullOptionalHashSet()
        {
            var result = GetCakeHost<Int32OptionalTask>().Run(FormatCustomArguments(nameof(Int32OptionalTask)));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32OptionalTask>()?.HashSet, Is.Not.Null);
        }

        [Test]
        public void CanRequireExternalArguments()
        {
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask)));

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exceptions = GetSprinklesExceptions();
            Assert.That(exceptions, Is.Not.Null.Or.Empty);
            Assert.Multiple(() =>
            {
                foreach (var expectedArgument in ExternalRequiredArguments)
                {
                    var exception = exceptions.FirstOrDefault(x => x.TaskArgumentName == expectedArgument);
                    Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotSet));
                }
            });
        }

        [Test]
        public void CanDecorateInternalArgumentsAlongsideExternalArguments()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.Single, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateExternalRequiredSingleWithLastValue()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.ExternalArguments.ExternalRequiredSingle, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateExternalRequiredList()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.ExternalArguments.ExternalRequiredList, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateExternalRequiredHashSet()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.ExternalArguments.ExternalRequiredHashSet, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateExternalOptionalSingleWithLastValue()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.ExternalArguments.ExternalOptionalSingle, Is.EqualTo(ArgumentValues.Last()));
        }

        [Test]
        public void CanDecorateExternalOptionalList()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.ExternalArguments.ExternalOptionalList, Is.EqualTo(ArgumentValues));
        }

        [Test]
        public void CanDecorateExternalOptionalHashSet()
        {
            var properties = PrepareArguments(ExternalArguments.SelectMany(x => ArgumentValues, (arg, value) => (arg, value.ToString())));
            var result = GetCakeHost<Int32ExternalArgumentsTask>().Run(FormatCustomArguments(nameof(Int32ExternalArgumentsTask), properties));

            Assert.That(result, Is.EqualTo(0), "Task failed.");
            Assert.That(GetContext<Int32ExternalArgumentsTask>()?.ExternalArguments.ExternalOptionalHashSet, Is.EqualTo(ArgumentValues));
        }
    }
}
