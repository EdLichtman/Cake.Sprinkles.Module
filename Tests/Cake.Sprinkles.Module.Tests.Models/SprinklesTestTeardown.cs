using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Frosting;

namespace Cake.Sprinkles.Module.Tests.Models
{
    public class SprinklesTestTeardown : IFrostingTeardown
    {
        public void Teardown(ICakeContext context, ITeardownContext info)
        {
            SprinklesTestContextProvider.Context = context;
            SprinklesTestContextProvider.ThrownException = info.ThrownException;
        }
    }
}
