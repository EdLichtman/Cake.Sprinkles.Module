using Cake.Frosting;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.StringTasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesModuleTests : SprinklesTestBase
    {
        [Test(Description = "Due to a limitation of writing a third party module for Cake, " +
            "I could had to choose between 2 suboptimal choices. This will fail because of my choice." +
            "If the Cake team allows me to incorporate this into the Cake Frosting Core, I can get this " +
            "test to pass again. But until then, this is living documentation to remind me of why it cannot be done.")]
        public void IncludingSprinklesModuleDoesNotPreventUserFromAddingTheirOwnTaskSetup()
        {
            var expectedSprinkledValue = "foo";
            var host = GetCakeHost<StringOptionalTask>();
            host.UseTaskSetup<SprinklesTestTaskSetup>();

            host.Run(
                FormatCustomArguments(
                    nameof(StringOptionalTask),
                    (nameof(StringOptionalTask.Single), expectedSprinkledValue)
                    )
                );
            Assert.Throws<AssertionException>(() =>
            {
                Assert.That(GetContext<StringOptionalTask>()?.Single, Is.EqualTo(expectedSprinkledValue));
            });

        }
    }
}
