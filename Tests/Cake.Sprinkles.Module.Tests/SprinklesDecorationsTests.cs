using Cake.Sprinkles.Module.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests
{
    [TestFixture]
    internal class SprinklesDecorationsTests
    {
        [Test]
        public void EnumeratedTypeOfListOfIntReturnsInt() {
            var expected = typeof(int);
            var actual = SprinklesDecorations.GetEnumeratedType(typeof(IList<int>));

            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}
