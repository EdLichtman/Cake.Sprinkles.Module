using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.BooleanTasks;
using Cake.Sprinkles.Module.Tests.Models.Int32;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesBooleanFlagTests : SprinklesTestBase
    {
        private CakeHost _host = null!;
        private BoolFlagTask? Context => (SprinklesTestContextProvider.Context as SprinklesTestContext<BoolFlagTask>)?.Task;

        [SetUp]
        public void SetUp()
        {
            _host = GetCakeHost<BoolFlagTask>();
        }

        [Test]
        public void CanPopulateBooleanViaFlag()
        {
            var result = _host.Run(new[]
            {
                "--target=Flag",
                "--flag"
            });

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            Assert.That(Context?.HasArgument, Is.EqualTo(true));
        }

        [Test]
        public void CanDecorateByNotIncludingFlag()
        {
            var result = _host.Run(new[]
            {
                "--target=Flag",
            });

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            Assert.That(Context?.HasArgument, Is.EqualTo(false));
        }

        [Test]
        public void CanForceFlagToBeFalse()
        {
            var result = _host.Run(new[]
            {
                "--target=Flag",
                "--flag=false"
            });

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            Assert.That(Context?.HasArgument, Is.EqualTo(false));
        }
    }
}
