using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Int32Tasks
{
    public class Int32ExternalArguments
    {
        [TaskArgumentName(nameof(ExternalRequiredSingle))]
        [TaskArgumentIsRequired]
        public int ExternalRequiredSingle { get; set; }

        [TaskArgumentName(nameof(ExternalRequiredList))]
        [TaskArgumentIsRequired]
        public ImmutableList<int> ExternalRequiredList { get; set; } = null!;

        [TaskArgumentName(nameof(ExternalRequiredHashSet))]
        [TaskArgumentIsRequired]
        public ImmutableHashSet<int> ExternalRequiredHashSet { get; set; } = null!;

        [TaskArgumentName(nameof(ExternalOptionalSingle))]
        public int ExternalOptionalSingle { get; set; }

        [TaskArgumentName(nameof(ExternalOptionalList))]
        public ImmutableList<int> ExternalOptionalList { get; set; } = null!;

        [TaskArgumentName(nameof(ExternalOptionalHashSet))]
        public ImmutableHashSet<int> ExternalOptionalHashSet { get; set; } = null!;
    }
}
