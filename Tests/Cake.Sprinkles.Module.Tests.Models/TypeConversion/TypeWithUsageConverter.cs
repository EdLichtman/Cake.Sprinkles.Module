using Cake.Sprinkles.Module.TypeConversion;

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
        protected override TypeWithUsage ConvertType(TaskArgument value)
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
        protected override TypeWithUsage ConvertType(TaskArgument value)
        {
            return new TypeWithUsage()
            {
                InternalProperty = PrependedText + value.GetValue()
            };
        }
    }

    public class TypeWithoutUsageConverter : TaskArgumentTypeConverter<TypeWithoutUsage>
    {
        protected override TypeWithoutUsage ConvertType(TaskArgument value)
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
        protected override TypeConversionThatErrors ConvertType(TaskArgument value)
        {
            throw new Exception(ExpectedInternalException);
        }
    }

    public class TypeWithUsageListConverter : TaskArgumentTypeConverter<IList<TypeWithUsage>>
    {
        protected override IList<TypeWithUsage> ConvertType(TaskArgument value)
        {
            return value.GetValues().Select(x => new TypeWithUsage
            {
                InternalProperty = x
            }).ToList();
        }
    }
}
