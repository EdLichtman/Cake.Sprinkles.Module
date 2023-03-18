using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.BooleanTasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesBooleanTaskTests : SprinklesTestBase
    {
        private CakeHost _host = null!;
        private BoolTask? Context => (SprinklesTestContextProvider.Context as SprinklesTestContext<BoolTask>)?.Task;
        //private Exception? ThrownException => ArgumentExtensionsContextProvider<Boolean>.ThrownException;

        [SetUp]
        public void SetUp()
        {
            _host = GetCakeHost<BoolTask>();
        }

        //[Test]
        //public void ThrowsErrorIfCastingIsInvalid()
        //{
        //    var properties = GetAllPropertiesAsStrings();
        //    var result = _host.Run(properties);

        //    Assert.That(result, Is.Not.EqualTo(1), "BooleanTask succeeded when it should have failed.");

        //    var exception = ThrownException;
        //    Assert.That(exception, Is.Not.Null);
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(exception!.Message,
        //            Does.Contain("is not a valid value for Boolean. (Parameter 'required_single')"));
        //        Assert.That(exception.Message,
        //            Does.Contain("is not a valid value for Boolean. (Parameter 'required_list')"));
        //        Assert.That(exception.Message,
        //            Does.Contain("is not a valid value for Boolean. (Parameter 'required_hashset')"));
        //        Assert.That(exception.Message,
        //            Does.Contain("is not a valid value for Boolean. (Parameter 'optional_single')"));
        //        Assert.That(exception.Message,
        //            Does.Contain("is not a valid value for Boolean. (Parameter 'optional_list')"));
        //        Assert.That(exception.Message,
        //            Does.Contain("is not a valid value for Boolean. (Parameter 'optional_hashset')"));
        //    });
        //}

        //[Test]
        //public void CanRequireMultipleArguments()
        //{
        //    var result = _host.Run(new String[] { });

        //    Assert.That(result, Is.Not.EqualTo(1), "BooleanTask succeeded when it should have failed.");

        //    var exception = ThrownException;
        //    Assert.That(exception, Is.Not.Null);
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(exception!.Message, Does.Contain("Argument 'required_single' was not set."));
        //        Assert.That(exception.Message, Does.Contain("Argument 'required_list' was not set."));
        //        Assert.That(exception.Message, Does.Contain("Argument 'required_hashset' was not set."));
        //    });
        //}

        [Test]
        public void CanDecorateRequiredSingle()
        {
            var arguments = GetAllPropertiesAsBooleans();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            var lastRequiredValue = Boolean.Parse(arguments.Last(x => x.Contains("required_single")).Split("=")[1]);

            Assert.That(Context!.RequiredSingle, Is.EqualTo(lastRequiredValue));
        }

        [Test]
        public void CanDecorateRequiredList()
        {
            var arguments = GetAllPropertiesAsBooleans();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            var requiredValues = arguments.Where(x => x.Contains("required_list"))
                .Select(x => Boolean.Parse(x.Split("=")[1])).ToList();

            Assert.That(Context!.RequiredList, Is.EqualTo(requiredValues));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var arguments = GetAllPropertiesAsBooleans();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            // A HashSet will only have 1 key for the same value.
            var requiredValues =
                arguments
                    .Where(x => x.Contains("required_hashset"))
                    .Select(x => Boolean.Parse(x.Split("=")[1]))
                    .Take(1)
                    .ToList();

            Assert.That(Context!.RequiredHashSet.OrderBy(x => x).ToList(), Is.EqualTo(requiredValues));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var arguments = GetAllPropertiesAsBooleans();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            var lastValue = Boolean.Parse(arguments.Last(x => x.Contains("optional_single")).Split("=")[1]);

            Assert.That(Context!.OptionalSingle, Is.EqualTo(lastValue));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var arguments = GetAllPropertiesAsBooleans();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            var values =
                arguments
                    .Where(x => x.Contains("optional_list"))
                    .Select(x => Boolean.Parse(x.Split("=")[1]))
                    .ToList();

            Assert.That(Context!.OptionalList, Is.EqualTo(values));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var arguments = GetAllPropertiesAsBooleans();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            // A HashSet will only have 1 key for the same value.
            var values =
                arguments
                    .Where(x => x.Contains("optional_hashset"))
                    .Select(x => Boolean.Parse(x.Split("=")[1]))
                    .Take(1)
                    .ToList();

            Assert.That(Context!.OptionalHashSet.OrderBy(x => x).ToList(), Is.EqualTo(values));
        }
    }
}