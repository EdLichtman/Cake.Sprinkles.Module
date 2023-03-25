using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TypeConversion
{
    [TaskName(nameof(TypeConverterInvalidTask))]
    public class TypeConverterInvalidTask : FrostingTask<SprinklesTestContext<TypeConverterInvalidTask>>
    {
        [TaskArgumentName(nameof(ConversionTypeWithUsage))]
        public TypeWithUsage ConversionTypeWithUsage { get; set; } = null!;

        [TaskArgumentName(nameof(TypeThatDoesNotNeedConversion))]
        public string TypeThatDoesNotNeedConversion { get; set; } = null!;

        [TaskArgumentName(nameof(TypeWithInvalidConverter))]
        [TaskArgumentConverter(typeof(Type))]
        public TypeWithUsage TypeWithInvalidConverter { get; set; } = null!;
        public override void Run(SprinklesTestContext<TypeConverterInvalidTask> context)
        {
            context.Task = this;
        }
    }
}
