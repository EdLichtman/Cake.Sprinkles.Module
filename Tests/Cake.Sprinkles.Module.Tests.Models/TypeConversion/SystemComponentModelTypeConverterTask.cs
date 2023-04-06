using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.TypeConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.TypeConversion
{
    [TypeConverter(typeof(MyCustomTypeConverter))]
    public class MyCustomType
    {
        public const string CustomExample = "Hello there. this is the usage.";
        public string? MyCustomValue { get; set; }

        public class MyCustomTypeConverter : TaskArgumentTypeConverter<MyCustomType>
        {
            public override IEnumerable<string> GetExampleInputValues()
            {
                yield return CustomExample;
            }
            protected override MyCustomType ConvertType(TaskArgument argument, CultureInfo? cultureInfo)
            {
                return new MyCustomType
                {
                    MyCustomValue = argument.GetValue()
                };
            }
        }
    }

    [TypeConverter(typeof(MyOtherCustomTypeConverter))]
    public class MyOtherCustomType
    {
        public string? MyCustomValue { get; set; }

        public class MyOtherCustomTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                return new MyOtherCustomType
                {
                    MyCustomValue = value.ToString()
                };
            }
        }
    }
    [TaskName(nameof(SystemComponentModelTypeConverterTask))]
    public class SystemComponentModelTypeConverterTask : FrostingTask<SprinklesTestContext<SystemComponentModelTypeConverterTask>>
    {
        [TaskArgumentName(nameof(MyCustomType))]
        public MyCustomType MyCustomType { get; set; } = null!;

        [TaskArgumentName(nameof(MyOtherCustomType))]
        public MyOtherCustomType MyOtherCustomType { get; set; } = null!;

        public override void Run(SprinklesTestContext<SystemComponentModelTypeConverterTask> context)
        {
            context.Task = this;
        }
    }
}
