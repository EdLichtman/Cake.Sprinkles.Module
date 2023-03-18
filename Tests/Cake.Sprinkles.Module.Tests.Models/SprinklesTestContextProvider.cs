using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Frosting;

namespace Cake.Sprinkles.Module.Tests.Models
{
    public static class SprinklesTestContextProvider
    {
        public static ICakeContext? Context { get; set; }

        public static Exception? ThrownException { get; set; }
    }
}
