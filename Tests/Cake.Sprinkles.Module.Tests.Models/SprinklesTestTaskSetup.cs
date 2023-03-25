using Cake.Core;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models
{
    public class SprinklesTestTaskSetup : IFrostingTaskSetup
    {
        public void Setup(ICakeContext context, ITaskSetupContext info)
        {
            // meant to test that we cannot override the SprinklesDescriptionTaskSetup
        }
    }
}
