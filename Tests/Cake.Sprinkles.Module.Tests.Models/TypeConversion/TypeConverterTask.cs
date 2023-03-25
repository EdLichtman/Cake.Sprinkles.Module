using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.TypeConversion;

namespace Cake.Sprinkles.Module.Tests.Models.TypeConversion
{
    [TaskName(nameof(TypeConverterTask))]
    public class TypeConverterTask : FrostingTask<SprinklesTestContext<TypeConverterTask>>
    {
        [TaskArgumentName(nameof(ConversionTypeWithUsage))]
        [TaskArgumentConverter(typeof(TypeWithUsageConverter))]
        public TypeWithUsage ConversionTypeWithUsage { get; set; } = null!;

        [TaskArgumentName(nameof(ConversionTypeWithUsageOther))]
        [TaskArgumentConverter(typeof(TypeWithUsageOtherConverter))]
        public TypeWithUsage ConversionTypeWithUsageOther { get; set; } = null!;

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
