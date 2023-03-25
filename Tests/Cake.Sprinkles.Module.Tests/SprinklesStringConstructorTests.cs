using Cake.Sprinkles.Module.Tests.Models.StringConstructorTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesStringConstructorTests : SprinklesTestBase
    {
        [Test]
        public void CanCreateDirectoryInfoUsingSingleStringConstructor()
        {
            var name = "Exist";
            var path = $@"C:\This\File\Does\Not\{name}";
            var arguments = FormatCustomArguments(
                nameof(StringConstructorTask),
                (nameof(StringConstructorTask.DirectoryInfo), path)
            );

            GetCakeHost<StringConstructorTask>().Run(arguments);

            var directory = GetContext<StringConstructorTask>()?.DirectoryInfo;
            Assert.IsNotNull(directory);
            Assert.That(directory.Exists, Is.False);
            Assert.That(directory.Name, Is.EqualTo(name));
        }

        [Test]
        public void CanCreateFileInfoUsingSingleStringConstructor()
        {
            var name = "Exist.txt";
            var path = $@"C:\This\File\Does\Not\{name}";
            var arguments = FormatCustomArguments(
                nameof(StringConstructorTask),
                (nameof(StringConstructorTask.FileInfo), path)
            );

            GetCakeHost<StringConstructorTask>().Run(arguments);

            var file = GetContext<StringConstructorTask>()?.FileInfo;
            Assert.IsNotNull(file);
            Assert.That(file.Exists, Is.False);
            Assert.That(file.Name, Is.EqualTo(name));
        }
    }
}
