using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.String;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesStringTests : SprinklesTestBase
    {
        private CakeHost _host = null!;
        private StringTask? Context => (SprinklesTestContextProvider.Context as SprinklesTestContext<StringTask>)?.Task;
        //private Exception? ThrownException => ArgumentExtensionsContextProvider<String>.ThrownException;

        [SetUp]
        public void SetUp()
        {
            _host = GetCakeHost<StringTask>();
        }

        //[Test]
        //public void CanRequireMultipleArguments()
        //{
        //    var result = _host.Run(new String[] { });

        //    Assert.That(result, Is.EqualTo(-1), "StringTask succeeded when it should have failed.");

        //    Assert.That(ThrownException, Is.Not.Null);
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(ThrownException!.Message, Does.Contain("Argument 'required_single' was not set."));
        //        Assert.That(ThrownException!.Message, Does.Contain("Argument 'required_list' was not set."));
        //        Assert.That(ThrownException!.Message, Does.Contain("Argument 'required_hashset' was not set."));
        //    });
        //}

        //[Test]
        //public void CanDecorateRequiredSingle()
        //{
        //    var arguments = GetAllPropertiesAsNumbers();
        //    var result = _host.Run(arguments);

        //    Assert.That(result, Is.EqualTo(0), "StringTask failed.");

        //    var lastRequiredString = arguments.Last(x => x.Contains("required_single")).Split("=")[1];

        //    Assert.That(Context?.RequiredSingle, Is.EqualTo(lastRequiredString));
        //}

        [Test]
        public void CanDecorateRequiredList()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");

            var lastRequiredStrings = arguments.Where(x => x.Contains("required_list")).Select(x => x.Split("=")[1]).ToList();

            Assert.That(Context?.RequiredList, Is.EqualTo(lastRequiredStrings));
        }

        [Test]
        public void CanDecorateRequiredHashSet()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");

            var lastRequiredStrings =
                arguments
                    .Where(x => x.Contains("required_hashset"))
                    .Select(x => x.Split("=")[1])
                    .OrderBy(x => x)
                    .ToList();

            Assert.That(Context?.RequiredHashSet.OrderBy(x => x).ToList(), Is.EqualTo(lastRequiredStrings));
        }

        [Test]
        public void CanDecorateOptionalSingle()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");

            var lastRequiredString = arguments.Last(x => x.Contains("optional_single")).Split("=")[1];

            Assert.That(Context?.OptionalSingle, Is.EqualTo(lastRequiredString));
        }

        [Test]
        public void CanDecorateOptionalList()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");

            var lastRequiredStrings = arguments.Where(x => x.Contains("optional_list")).Select(x => x.Split("=")[1]).ToList();

            Assert.That(Context?.OptionalList, Is.EqualTo(lastRequiredStrings));
        }

        [Test]
        public void CanDecorateOptionalHashSet()
        {
            var arguments = GetAllPropertiesAsNumbers();
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");

            var lastRequiredStrings = 
                arguments
                    .Where(x => x.Contains("optional_hashset"))
                    .Select(x => x.Split("=")[1])
                    .OrderBy(x => x)
                    .ToList();

            Assert.That(Context?.OptionalHashSet.OrderBy(x => x).ToList(), Is.EqualTo(lastRequiredStrings));
        }

        [Test]
        public void OptionalListIsInstantiatedEvenWhenNoArgumentsProvided()
        {
            var arguments = GetAllPropertiesAsStrings(true);
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");
            Assert.Multiple(() =>
            {
                Assert.That(Context?.OptionalList, Is.Not.Null);
                Assert.That(Context?.OptionalList, Is.Empty);
            });
        }

        [Test]
        public void OptionalHashSetIsInstantiatedEvenWhenNoArgumentsProvided()
        {
            var arguments = GetAllPropertiesAsStrings(true);
            var result = _host.Run(arguments);

            Assert.That(result, Is.EqualTo(0), "StringTask failed.");
            Assert.Multiple(() =>
            {
                Assert.That(Context?.OptionalHashSet, Is.Not.Null);
                Assert.That(Context?.OptionalHashSet, Is.Empty);
            });
        }
    }
}
