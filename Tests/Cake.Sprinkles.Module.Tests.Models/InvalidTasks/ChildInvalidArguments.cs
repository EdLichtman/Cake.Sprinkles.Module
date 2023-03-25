using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.InvalidTasks
{
    public class ChildInvalidArguments
    {
        [TaskArgumentName(InvalidTask.MyDuplicateArgumentName)]
        public string ChildArgument { get; set; } = null!;

        [TaskArgumentName(nameof(RequiredChildArgument))]
        [TaskArgumentDescription("The purpose of this is to demonstrate that when a child has multiple exceptions, we flatten them all into one single AggregateException")]
        [TaskArgumentIsRequired]
        public string RequiredChildArgument { get; set; } = null!;

        [TaskArgumentName(nameof(OtherRequiredChildArgument))]
        [TaskArgumentDescription("The purpose of this is to demonstrate that when a child has multiple exceptions, we flatten them all into one single AggregateException")]
        [TaskArgumentIsRequired]
        public string OtherRequiredChildArgument { get; set; } = null!;

        [TaskArguments]
        public ChildInvalidArgumentsOneDeep ChildInvalidArgumentsOneDeep { get; set; } = null!;
    }
}
