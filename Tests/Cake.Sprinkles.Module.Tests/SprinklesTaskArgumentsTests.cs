using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Tests.Models.Int32Tasks;
using Cake.Sprinkles.Module.Tests.Models.TaskArgumentsTasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesTaskArgumentsTests : SprinklesTestBase
    {
        [Test]
        public void MultipleLevelsOfHierarchyAreSetOnExternalTaskArguments()
        {
            RunCakeHost<TaskArgumentsTask>(
                (nameof(TaskArgumentsLevelOne.LevelOneProperty), nameof(TaskArgumentsLevelOne.LevelOneProperty)),
                (nameof(TaskArgumentsLevelTwo.LevelTwoProperty), nameof(TaskArgumentsLevelTwo.LevelTwoProperty)),
                (nameof(TaskArgumentsLevelThree.LevelThreeProperty), nameof(TaskArgumentsLevelThree.LevelThreeProperty)));

            var context = GetContext<TaskArgumentsTask>();
            Assert.That(
                context?.TaskArgumentsLevelOne?.LevelOneProperty, 
                Is.EqualTo(nameof(TaskArgumentsLevelOne.LevelOneProperty)));

            Assert.That(
                context?.TaskArgumentsLevelOne?.TaskArgumentsLevelTwo.LevelTwoProperty, 
                Is.EqualTo(nameof(TaskArgumentsLevelTwo.LevelTwoProperty)));

            Assert.That(
                context?.TaskArgumentsLevelOne?.TaskArgumentsLevelTwo.TaskArgumentsLevelThree.LevelThreeProperty, 
                Is.EqualTo(nameof(TaskArgumentsLevelThree.LevelThreeProperty)));
        }

        [Test]
        public void CanParseArgumentsInTaskArguments()
        {
            var expectedValue = 1;
            RunCakeHost<TaskArgumentsTask>(
                (nameof(TaskArgumentsLevelOne.LevelOneIntProperty), expectedValue.ToString()));

            var context = GetContext<TaskArgumentsTask>();

            Assert.That(
                context?.TaskArgumentsLevelOne?.LevelOneIntProperty,
                Is.EqualTo(expectedValue));
        }     
    }
}
