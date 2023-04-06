using Cake.Sprinkles.Module.TypeConversion;
using System.Globalization;

namespace Cake.Sprinkles.Module.Tests.Models.TypeConversion
{
    public class TypeWithUsageConverter : TaskArgumentTypeConverter<TypeWithUsage>
    {
        public static IEnumerable<string> GetUsageValuesStatic()
        {
            yield return "foo";
            yield return "bar";
        }
        public override IEnumerable<string> GetExampleInputValues()
        {
            return GetUsageValuesStatic();
        }
        protected override TypeWithUsage ConvertType(TaskArgument value, CultureInfo? cultureInfo)
        {
            return new TypeWithUsage()
            {
                InternalProperty = value.GetValue()
            };
        }
    }

    public class TypeWithUsageOtherConverter : TaskArgumentTypeConverter<TypeWithUsage>
    {
        public const string PrependedText = "MyTextToPrepend";
        public static IEnumerable<string> GetUsageValuesStatic()
        {
            yield return "foo";
            yield return "bar";
        }
        public override IEnumerable<string> GetExampleInputValues()
        {
            return GetUsageValuesStatic();
        }
        protected override TypeWithUsage ConvertType(TaskArgument value, CultureInfo? cultureInfo)
        {
            return new TypeWithUsage()
            {
                InternalProperty = PrependedText + value.GetValue()
            };
        }
    }

    public class TypeWithoutUsageConverter : TaskArgumentTypeConverter<TypeWithoutUsage>
    {
        protected override TypeWithoutUsage ConvertType(TaskArgument value, CultureInfo? cultureInfo)
        {
            return new TypeWithoutUsage()
            {
                InternalProperty = value.GetValue()
            };
        }
    }

    public class TypeConversionThatErrorsTypeConverter : TaskArgumentTypeConverter<TypeConversionThatErrors>
    {
        public const string ExpectedInternalException = "Something went wrong while converting type.";
        protected override TypeConversionThatErrors ConvertType(TaskArgument value, CultureInfo? cultureInfo)
        {
            throw new Exception(ExpectedInternalException);
        }
    }

    public class TypeWithUsageListConverter : TaskArgumentTypeConverter<IList<TypeWithUsage>>
    {
        protected override IList<TypeWithUsage> ConvertType(TaskArgument value, CultureInfo? cultureInfo)
        {
            return value.GetValues().Select(x => new TypeWithUsage
            {
                InternalProperty = x
            }).ToList();
        }
    }
}
