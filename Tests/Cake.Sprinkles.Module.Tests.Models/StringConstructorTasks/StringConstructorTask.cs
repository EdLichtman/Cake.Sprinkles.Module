using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.StringConstructorTasks
{
    [TaskName(nameof(StringConstructorTask))]
    public class StringConstructorTask : FrostingTask<SprinklesTestContext<StringConstructorTask>>
    {
        [TaskArgumentName(nameof(DirectoryInfo))]
        public DirectoryInfo DirectoryInfo { get; set; } = null!;

        [TaskArgumentName(nameof(FileInfo))]
        public FileInfo FileInfo { get; set; } = null!;

        public override void Run(SprinklesTestContext<StringConstructorTask> testContext)
        {
            testContext.Task = this;
        }
    }
}
