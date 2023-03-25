using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TypeConversion
{
    public class TypeWithUsage
    {
        public string? InternalProperty { get; set; }
    }

    public class TypeWithoutUsage
    {
        public string? InternalProperty { get; set; }
    }

    public class TypeConversionThatErrors
    {

    }
}
