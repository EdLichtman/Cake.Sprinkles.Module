using Cake.Sprinkles.Module.Tests.Models.EnumTasks;
using Cake.Sprinkles.Module.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesEnumTests : SprinklesTestBase
    {
        [Test]
        public void CanRejectInvalidEnum()
        {
            RunCakeHost<EnumTask>(
                (nameof(EnumTask.TestEnum), "foo"));

            var context = GetContext<EnumTask>();
            Assert.That(context?.TestEnum, Is.EqualTo(default));

            var exception = GetSprinklesExceptionForProperty<EnumTask>(nameof(EnumTask.TestEnum));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception?.InnerMessage, Is.EqualTo(SprinklesValidator.Message_ArgumentWasNotAValidValueForType));
        }

        [Test]
        public void CanAcceptValidEnumString()
        {
            var expectedValue = TestEnum.Friday;
            RunCakeHost<EnumTask>(
                (nameof(EnumTask.TestEnum), expectedValue.ToString()));

            var context = GetContext<EnumTask>();
            Assert.That(context?.TestEnum, Is.EqualTo(expectedValue));
        }


        [Test]
        public void CanAcceptValidEnumInt()
        {
            var expectedValue = TestEnum.Friday;
            RunCakeHost<EnumTask>(
                (nameof(EnumTask.TestEnum), ((int)expectedValue).ToString()));

            var context = GetContext<EnumTask>();
            Assert.That(context?.TestEnum, Is.EqualTo(expectedValue));
        }
    }
}
