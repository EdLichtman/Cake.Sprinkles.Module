using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Validation
{
    [TaskName(nameof(ValidationTask))]
    public class ValidationTask : FrostingTask<SprinklesTestContext<ValidationTask>>
    {
        [TaskArgumentName(nameof(ThrowsExceptionInValidator))]
        [ThrowsErrorValidation]
        public string ThrowsExceptionInValidator { get; set; } = null!;

        [TaskArgumentName(nameof(ThrowsExceptionIfMoreThanFive))]
        [ValidateIntegerLessThanFive]
        public int ThrowsExceptionIfMoreThanFive { get; set; }

        [TaskArgumentName(nameof(AllowsMoreThanOneValidator))]
        [ValidateIntegerGreaterThanOne]
        [ValidateIntegerLessThanFive]
        public int AllowsMoreThanOneValidator { get; set; }

        [TaskArgumentName(nameof(EnumerableValidation))]
        [ValidateIntegerLessThanFive]
        public ImmutableList<int> EnumerableValidation { get; set; } = null!;

        public override void Run(SprinklesTestContext<ValidationTask> context)
        {
            context.Task = this;
        }
    }
}
