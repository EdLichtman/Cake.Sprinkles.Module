using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models.BooleanTasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesBooleanFlagTests : SprinklesTestBase
    {
        private CakeHost _host = null!;
        private BoolFlagTask? Context => GetContext<BoolFlagTask>();

        [SetUp]
        public void SetUp()
        {
            _host = GetCakeHost<BoolFlagTask>();
        }

        [Test]
        public void CanPopulateBooleanViaFlag()
        {
            var result = _host.Run(
                FormatCustomArguments(
                    nameof(BoolFlagTask), 
                    (nameof(BoolFlagTask.HasArgument), null))
                );

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            Assert.That(Context?.HasArgument, Is.EqualTo(true));
        }

        [Test]
        public void CanDecorateByNotIncludingFlag()
        {
            var result = _host.Run(
                 FormatCustomArguments(
                    nameof(BoolFlagTask))
                );

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            Assert.That(Context?.HasArgument, Is.EqualTo(false));
        }

        [Test]
        public void CanForceFlagToBeFalse()
        {
            var result = _host.Run(
                FormatCustomArguments(
                    nameof(BoolFlagTask),
                    (nameof(BoolFlagTask.HasArgument), false.ToString()))
                );

            Assert.That(result, Is.EqualTo(0), "BooleanTask failed.");

            Assert.That(Context?.HasArgument, Is.EqualTo(false));
        }
    }
}
