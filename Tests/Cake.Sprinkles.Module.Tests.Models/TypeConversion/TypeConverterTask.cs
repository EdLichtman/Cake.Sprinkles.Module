using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;

namespace Cake.Sprinkles.Module.Tests.Models.TypeConversion
{
    [TaskName(nameof(TypeConverterTask))]
    public class TypeConverterTask : FrostingTask<SprinklesTestContext<TypeConverterTask>>
    {
        [TaskArgumentName(nameof(ConversionTypeWithUsage))]
        public TypeWithUsage ConversionTypeWithUsage { get; set; } = null!;

        [TaskArgumentName(nameof(OtherConversionType))]
        public TypeWithoutUsage OtherConversionType { get; set; } = null!;

        [TaskArgumentName(nameof(TypeConversionThatErrors))]
        public TypeConversionThatErrors TypeConversionThatErrors { get; set; } = null!;

        [TaskArgumentName(nameof(ConversionTypeWithListUsage))]
        public IList<TypeWithUsage> ConversionTypeWithListUsage { get; set; } = null!;

        public override void Run(SprinklesTestContext<TypeConverterTask> context)
        {
            context.Task = this;
        }

    }
}
