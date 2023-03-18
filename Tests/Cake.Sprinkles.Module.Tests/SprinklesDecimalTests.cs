using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.BooleanTasks;
using Cake.Sprinkles.Module.Tests.Models.DecimalTasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesDecimalTests : SprinklesTestBase
    {
        private CakeHost _host = null!;
        private Int32Task? Context => (SprinklesTestContextProvider.Context as SprinklesTestContext<DecimalTask>)?.Task;
        private Exception? ThrownException => SprinklesTestContextProvider.ThrownException;

        [SetUp]
        public void SetUp()
        {
            _host = GetCakeHost<DecimalTask>();
        }

        [Test]
        public void ThrowsErrorIfCastingIsInvalid()
        {
            var properties = GetAllPropertiesAsStrings();
            var result = _host.Run(properties);

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exception = ThrownException;
            Assert.That(exception, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(exception!.Message, Does.Contain("is not a valid value for Decimal. (Parameter 'required_single')"));
                Assert.That(exception.Message, Does.Contain("is not a valid value for Decimal. (Parameter 'required_list')"));
                Assert.That(exception.Message, Does.Contain("is not a valid value for Decimal. (Parameter 'required_hashset')"));
                Assert.That(exception.Message, Does.Contain("is not a valid value for Decimal. (Parameter 'optional_single')"));
                Assert.That(exception.Message, Does.Contain("is not a valid value for Decimal. (Parameter 'optional_list')"));
                Assert.That(exception.Message, Does.Contain("is not a valid value for Decimal. (Parameter 'optional_hashset')"));
            });
        }

        [Test]
        public void CanRequireMultipleArguments()
        {
            var result = _host.Run(new String[] { });

            Assert.That(result, Is.EqualTo(-1), "Task succeeded when it should have failed.");

            var exception = ThrownException;
            Assert.That(exception, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(exception!.Message, Does.Contain("Argument 'required_single' was not set."));
                Assert.That(exception.Message, Does.Contain("Argument 'required_list' was not set."));
                Assert.That(exception.Message, Does.Contain("Argument 'required_hashset' was not set."));
            });
        }

        [Test]
        public void CanDecorateRequiredSingle()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "Task failed.");

            var lastRequiredValue = Decimal.Parse(arguments.Last(x => x.Contains("required_single")).Split("=")[1]);

            Assert.That(Context?.RequiredSingle, Is.EqualTo(lastRequiredValue));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "Task failed.");

            var requiredValues = arguments.Where(x => x.Contains("required_list")).Select(x => Decimal.Parse(x.Split("=")[1])).ToList();

            Assert.That(Context?.RequiredList, Is.EqualTo(requiredValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "Task failed.");

            var requiredValues =
                arguments
                    .Where(x => x.Contains("required_hashset"))
                    .Select(x => Decimal.Parse(x.Split("=")[1]))
                    .OrderBy(x => x)
                    .ToList();

            Assert.That(Context?.RequiredHashSet.OrderBy(x => x).ToList(), Is.EqualTo(requiredValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "Task failed.");

            var lastValue = Decimal.Parse(arguments.Last(x => x.Contains("optional_single")).Split("=")[1]);

            Assert.That(Context?.OptionalSingle, Is.EqualTo(lastValue));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "Task failed.");

            var values = 
                arguments
                    .Where(x => x.Contains("optional_list"))
                    .Select(x => Decimal.Parse(x.Split("=")[1]))
                    .ToList();

            Assert.That(Context?.OptionalList, Is.EqualTo(values));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "Task failed.");

            var values =
                arguments
                    .Where(x => x.Contains("optional_hashset"))
                    .Select(x => Decimal.Parse(x.Split("=")[1]))
                    .OrderBy(x => x)
                    .ToList();

            Assert.That(Context?.OptionalHashSet.OrderBy(x => x).ToList(), Is.EqualTo(values));
        }
    }
}
