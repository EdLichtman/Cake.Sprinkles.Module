using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.InvalidTasks
{
    public class ParameterConstructorClassChildArguments
    {
        private readonly string _foo;
        public ParameterConstructorClassChildArguments(string foo)
        {
            _foo = foo;
        }

        public string ObligatoryArgumentOnParameterConstructor { get; set; } = null!;
    }

    
}
