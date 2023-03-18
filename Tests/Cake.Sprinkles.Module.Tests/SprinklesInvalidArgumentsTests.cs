using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests
{
    internal class SprinklesInvalidArgumentsTests
    {
        [Test]
        public void ThrowsExceptionIfTaskContainsEnumerableThatIsNotImmutableListOrHashSet()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagButNoBoolean()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ThrowsExceptionIfTaskArgumentFlagIsRequired()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ThrowsExceptionIfTaskContainsFlagThatIsEnumerable()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddsDescriptionToRequiredArgument()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        public void ThrowsExceptionIfInputHasDelimiterAndMoreThanOneArgumentPassedIn()
        {
            throw new NotImplementedException(); 
        }

        [Test]
        public void ThrowsExceptionIfDelimiterIsSetOnArgumentWithNoEnumeration()
        {
            throw new NotImplementedException(); 
        }

        [Test]
        public void ThrowsExceptionIfDelimiterIsSetAndMoreThanOneArgumentIsPassedIn()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ErrorThrownBySprinklesWithNoDescriptionHasNoDescriptionInMessage()
        {
            throw new NotImplementedException();
        }
    }
}
