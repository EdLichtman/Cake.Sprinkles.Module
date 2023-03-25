using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.InvalidTasks
{
    public struct ParameterConstructorStructChildArguments
    {
        private readonly string _foo;
        public ParameterConstructorStructChildArguments(string foo)
        {
            _foo = foo;
        }

        public string ObligatoryArgumentOnParameterConstructorStruct { get; set; } = null!;
    }
}
