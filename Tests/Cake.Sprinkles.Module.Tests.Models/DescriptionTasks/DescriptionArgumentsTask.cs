using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using Cake.Sprinkles.Module.Tests.Models.TypeConversion;
using Cake.Sprinkles.Module.TypeConversion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.DescriptionTasks
{
    [TaskName(nameof(DescriptionArgumentsTask))]
    [TaskDescription($"{nameof(TaskDescriptionAttribute)}{nameof(DescriptionArgumentsTask)}")]
    public class DescriptionArgumentsTask : FrostingTask
    {
        public const string UsageValue1 = nameof(UsageValue1);
        public const string UsageValue2 = nameof(UsageValue2);

        [TaskArgumentName(nameof(DescribedValue))]
        [TaskArgumentDescription($"{nameof(TaskArgumentDescriptionAttribute)}{nameof(DescribedValue)}")]
        public string DescribedValue { get; set; } = null!;

        [TaskArgumentName(nameof(RequiredValue))]
        [TaskArgumentIsRequired]
        public string RequiredValue { get; set; } = null!;

        [TaskArgumentName(nameof(IntValue))]
        public int IntValue { get; set; }

        [TaskArgumentName(nameof(FlagValue))]
        [TaskArgumentIsFlag]
        public bool FlagValue { get; set; }

        [TaskArgumentName(nameof(EnumerableValue))]
        public ImmutableList<string> EnumerableValue { get; set; } = null!;

        [TaskArgumentName(nameof(DelimiterValue))]
        [TaskArgumentEnumerationDelimiter(nameof(DelimiterValue))]
        public ImmutableList<string> DelimiterValue { get; set; } = null!;

        [TaskArgumentName(nameof(UsageValue))]
        [TaskArgumentExampleValue(UsageValue1)]
        [TaskArgumentExampleValue(UsageValue2)]
        public string UsageValue { get; set; } = null!;

        [TaskArguments]
        public DescriptionArguments Arguments { get; set; } = null!;
        
        [TaskArgumentName(nameof(DescriptionEnum))]
        public DescriptionEnum DescriptionEnum { get; set; }
    }
}
