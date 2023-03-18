using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Frosting;

namespace Cake.Sprinkles.Module.Tests.Models
{
    public class SprinklesTestContext<TFrostingTask> : FrostingContext where TFrostingTask : IFrostingTask
    {
        public TFrostingTask? Task { get; set; } 

        public SprinklesTestContext(ICakeContext context) : base(context)
        {
        }
    }
}
