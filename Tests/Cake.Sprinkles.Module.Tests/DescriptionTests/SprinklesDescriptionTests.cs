using Cake.Core;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Tests.Models.DescriptionTasks;
using Cake.Sprinkles.Module.Tests.Models.InvalidTasks;
using Cake.Sprinkles.Module.Tests.Models.StringTasks;
using Cake.Sprinkles.Module.Tests.Models.TypeConversion;
using Cake.Sprinkles.Module.Tests.Models.Validation;
using Cake.Sprinkles.Module.TypeConversion;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.DescriptionTests
{
    [TestFixture]
    internal class SprinklesDescriptionTests : SprinklesTestBase
    {
        private ConsoleReader? console = null;

        [TearDown]
        public void TearDown()
        {
            if (console != null)
            {
                console.Dispose();
            }
        }

        [Test(Description =
            "Tests should normally do only one thing, but the sheer definition of such a test, to make sure documentation " +
            "is stable, requires we break this rule. This is a bad example of a test, but a necessary one.")]
        public void DescriptionWithNoTargetBringsBackAllTargets()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(false);

            Assert.IsNull(console.GetIndex(RegexProvider.DependencyTreeMessage, true));

            // Not using Assert.Multiple because when we aggregate it, and there's an error, we can't see the stack trace and know where the error is.
            var headerTitleIndex = console.GetIndex(RegexProvider.TaskDescriptionHeader);
            var headerSpacerLines = console.GetIndices(RegexProvider.HeaderSeparater);
            Assert.That(headerSpacerLines, Does.Contain(headerTitleIndex + 1));

            var categorizedTaskIndex = console.GetIndex(RegexProvider.GetNoTargetOutputForTask<NoArgumentTask>());
            Assert.That(categorizedTaskIndex, Is.GreaterThan(headerTitleIndex));

            var uncategorizedTaskIndex = console.GetIndex(RegexProvider.GetNoTargetOutputForTask<DescriptionArgumentsTask>());
            Assert.That(uncategorizedTaskIndex, Is.GreaterThan(headerTitleIndex));

            var runThisCommandFurtherIndex = console.GetIndex(RegexProvider.RunCommandWithTarget);

            Assert.That(headerSpacerLines, Does.Contain(runThisCommandFurtherIndex - 1));
        }

        [Test(Description =
            "Tests should normally do only one thing, but the sheer definition of such a test, to make sure documentation " +
            "is stable, requires we break this rule. This is a bad example of a test, but a necessary one.")]
        public void DescriptionWithBarrenTargetBringsBackOnlyBarrenTarget()
        {
            console = GetConsoleReader<NoArgumentTask>(true);

            // Not using Assert.Multiple because when we aggregate it, and there's an error, we can't see the stack trace and know where the error is.
            Assert.IsNull(console.GetIndex(RegexProvider.TaskDescriptionHeader, true));
            Assert.IsNull(console.GetIndex(RegexProvider.GetTargetTaskName<DescriptionArgumentsTask>(), true));
            Assert.IsNull(console.GetIndex(RegexProvider.RunCommandWithTarget, true));
            Assert.IsNull(console.GetIndex(RegexProvider.DependencyTreeMessage, true));

            var categorizedTaskNameIndex = console.GetIndex(RegexProvider.GetTargetTaskName<NoArgumentTask>());

            var categorizedTaskDescriptionIndex = console.GetIndex(RegexProvider.GetTargetTaskDescription<NoArgumentTask>());
            Assert.That(categorizedTaskDescriptionIndex, Is.EqualTo(categorizedTaskNameIndex + 1));

            var noArgumentsAvailableIndex = console.GetIndex(RegexProvider.NoArgumentsAvailable);
            Assert.That(noArgumentsAvailableIndex, Is.GreaterThan(categorizedTaskDescriptionIndex));
        }

        [Test(Description =
            "Tests should normally do only one thing, but the sheer definition of such a test, to make sure documentation " +
            "is stable, requires we break this rule. This is a bad example of a test, but a necessary one.")]
        public void DescriptionWithDescriptiveTargetBringsBackOnlyDescriptiveTarget()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);

            // Not using Assert.Multiple because when we aggregate it, and there's an error, we can't see the stack trace and know where the error is.

            // Validate the blacklist of lines for this task description.
            Assert.IsNull(console.GetIndex(RegexProvider.TaskDescriptionHeader, true));
            Assert.IsNull(console.GetIndex(RegexProvider.GetNoTargetOutputForTask<NoArgumentTask>(), true));
            Assert.IsNull(console.GetIndex(RegexProvider.GetTargetTaskName<NoArgumentTask>(), true));
            Assert.IsNull(console.GetIndex(RegexProvider.GetTargetTaskDescription<NoArgumentTask>(), true));
            Assert.IsNull(console.GetIndex(RegexProvider.RunCommandWithTarget, true));
            Assert.IsNull(console.GetIndex(RegexProvider.NoArgumentsAvailable, true));
            Assert.IsNull(console.GetIndex(RegexProvider.GetArgument(string.Empty), true));
            Assert.IsNull(console.GetIndex(RegexProvider.DependencyTreeMessage, true));
            Assert.IsNull(console.GetIndex(RegexProvider.GetValidates(".*"), true));

            var emptyDescriptionsCount = console.GetIndices(RegexProvider.EmptyDescription).Count;
            Assert.That(emptyDescriptionsCount, Is.EqualTo(0), $"Expected no empty descriptions, but found: {emptyDescriptionsCount}. Also, this is the format test.{{0}}");

            // Validate the task, and that the description shows up as expected.
            var categorizedTaskNameIndex = console.GetIndex(RegexProvider.GetTargetTaskName<DescriptionArgumentsTask>());
            var categorizedTaskDescriptionIndex = console.GetIndex(RegexProvider.GetTargetTaskDescription<DescriptionArgumentsTask>());
            Assert.That(categorizedTaskDescriptionIndex, Is.EqualTo(categorizedTaskNameIndex + 1));

            // Get the index of the "following arguments" headers
            var requiredArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsRequired);
            var optionalArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsOptional);

            Assert.That(requiredArgumentsHeaderIndex, Is.GreaterThan(categorizedTaskDescriptionIndex));
            Assert.That(optionalArgumentsHeaderIndex, Is.GreaterThan(requiredArgumentsHeaderIndex));

            // prepare regexes for sanity
            var requiredValue = nameof(DescriptionArgumentsTask.RequiredValue);
            var describedValue = nameof(DescriptionArgumentsTask.DescribedValue);
            var intValue = nameof(DescriptionArgumentsTask.IntValue);
            var flagValue = nameof(DescriptionArgumentsTask.FlagValue);
            var enumerableValue = nameof(DescriptionArgumentsTask.EnumerableValue);
            var delimiterValue = nameof(DescriptionArgumentsTask.DelimiterValue);
            var usageValue = nameof(DescriptionArgumentsTask.UsageValue);
            var externalValue = nameof(DescriptionArguments.ExternalArgument);
            var externalValueChild = nameof(DescriptionArgumentsChild.ChildExternalArgument);
            var enumValue = nameof(DescriptionArgumentsTask.DescriptionEnum);

            // Check to make sure regexes are in the correct category: required or optional
            console.ConfirmLineBetween(
                requiredArgumentsHeaderIndex,
                RegexProvider.GetArgument(requiredValue),
                optionalArgumentsHeaderIndex);

            console.ConfirmLineBetween(
                requiredArgumentsHeaderIndex,
                RegexProvider.GetArgument(externalValueChild),
                optionalArgumentsHeaderIndex);

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(describedValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(intValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(flagValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(enumerableValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(delimiterValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(usageValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(externalValue));

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(enumValue));

            // Check to make sure each run is what we expect
            console.ConfirmRunContains(
                RegexProvider.GetArgument(requiredValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(requiredValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(externalValueChild),
                RegexProvider.GetArgumentDescription<DescriptionArgumentsChild>(externalValueChild),
                RegexProvider.GetAcceptsType<DescriptionArgumentsChild>(externalValueChild),
                RegexProvider.GetUsage<DescriptionArgumentsChild>(nameof(DescriptionArgumentsChild.ChildExternalArgument)));

            // Now, validate the Optional arguments
            console.ConfirmRunContains(
                RegexProvider.GetArgument(describedValue),
                RegexProvider.GetArgumentDescription<DescriptionArgumentsTask>(describedValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(describedValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(intValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(intValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(flagValue),
                RegexProvider.CanBeFlag,
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(flagValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(enumerableValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(enumerableValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(delimiterValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(delimiterValue),
                RegexProvider.GetRecognizesDelimiter<DescriptionArgumentsTask>(delimiterValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(usageValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(usageValue),
                RegexProvider.GetRawUsage($"--{usageValue}={DescriptionArgumentsTask.UsageValue1}"),
                RegexProvider.GetRawUsage($"--{usageValue}={DescriptionArgumentsTask.UsageValue2}"));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(externalValue),
                RegexProvider.GetAcceptsType<DescriptionArguments>(externalValue));

            console.ConfirmRunContains(
                RegexProvider.GetArgument(enumValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(enumValue),
                new Regex("^ *\\* " + DescriptionEnum.EnumOne + " *$"),
                new Regex("^ *\\* " + DescriptionEnum.FightingMongooses + " *$")
                );
        }

        [Test]
        public void DescriptionWithInvalidTaskWillThrowErrors()
        {
            console = GetConsoleReader<InvalidTask>(true);

            Assert.IsNull(console.GetIndex(RegexProvider.DependencyTreeMessage, true));
            
            //Since there is at least one string property that doesn't elicit any failures, make sure " * Accepts: String" doesn't show up.
            Assert.IsNull(console.GetIndex(RegexProvider.GetAcceptsType<string>(), true));

            var compileError = console.GetIndex(RegexProvider.CompileErrorOccurred);
            Assert.That(compileError, Is.Not.Null);

            // test for a sprinkling of errors
            console.ConfirmAllLinesBetween(
                compileError,
                new Regex(".*enumerable Argument must implement ImmutableList or ImmutableHashSet.*"));

            console.ConfirmAllLinesBetween(
                compileError,
                new Regex(".*flag cannot be a required argument.*"));

            console.ConfirmAllLinesBetween(
                compileError,
                new Regex(".*parameterless constructor.*"));

            console.ConfirmAllLinesBetween(
                compileError,
                new Regex(".*cannot appear more than one time on a task.*"));

            console.ConfirmAllLinesBetween(
                compileError,
                new Regex(".*Argument cannot be a struct.*"));
        }

        [Test]
        public void DescriptionWithDependencyRevealsTaskHasDependency()
        {
            console = GetConsoleReader<WithDependencyTask>(true);

            var noArgumentsAvailableIndex = console.GetIndex(RegexProvider.NoArgumentsAvailable);
            var dependencyMessage = console.GetIndex(RegexProvider.DependencyTreeMessage);

            Assert.That(dependencyMessage, Is.GreaterThan(noArgumentsAvailableIndex));
        }


        [Test]
        public void DescriptionWithDependencyFromDependeeRevealsTaskHasDependency()
        {
            console = GetConsoleReader<WithDependencyTask>(true);

            var noArgumentsAvailableIndex = console.GetIndex(RegexProvider.NoArgumentsAvailable);
            var dependencyMessage = console.GetIndex(RegexProvider.DependencyTreeMessage);

            Assert.That(dependencyMessage, Is.GreaterThan(noArgumentsAvailableIndex));
        }

        [Test]
        public void DescriptionOfTaskArgumentValidationIncludesDescriptionOfValidation()
        {
            console = GetConsoleReader<ValidationTask>(true);

            console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(ValidationTask.AllowsMoreThanOneValidator)),
                RegexProvider.GetAcceptsType<ValidationTask>(nameof(ValidationTask.AllowsMoreThanOneValidator)),
                RegexProvider.GetValidates(ValidateIntegerLessThanFiveAttribute.DescriptionOfValidationConstant),
                RegexProvider.GetValidates(ValidateIntegerGreaterThanOneAttribute.DescriptionOfValidationConstant));
        }

        [Test]
        public void DescriptionIncludesUsagesForTaskArgumentTypeConverter()
        {
            console = GetConsoleReader<TypeConverterTask>(true, 
                (cakeHost) => cakeHost
                    .RegisterTypeConverter<TypeWithUsageConverter>()
                    .RegisterTypeConverter<TypeWithoutUsageConverter>());

            var runLines = new List<Regex?>
            {
                RegexProvider.GetAcceptsType<TypeConverterTask>(nameof(TypeConverterTask.ConversionTypeWithUsage))
            };

            foreach (var usage in TypeWithUsageConverter.GetUsageValuesStatic())
            {
                runLines.Add(RegexProvider.GetRawUsage($".*={usage}"));
            }
            
            console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(TypeConverterTask.ConversionTypeWithUsage)),
                runLines.ToArray());
        }

        [Test]
        public void DescriptionIncludesUsagesWithTaskArgumentIdentifiersForTaskArgumentTypeConverter()
        {
            console = GetConsoleReader<TypeConverterTask>(true,
                (cakeHost) => cakeHost
                    .RegisterTypeConverter<TypeWithUsageConverter>()
                    .RegisterTypeConverter<TypeWithoutUsageConverter>());

            var runLines = new List<Regex?>
            {
                RegexProvider.GetAcceptsType<TypeConverterTask>(nameof(TypeConverterTask.ConversionTypeWithUsage))
            };

            foreach (var usage in TypeWithUsageConverter.GetUsageValuesStatic())
            {
                runLines.Add(RegexProvider.GetRawUsage($"--{nameof(TypeConverterTask.ConversionTypeWithUsage)}={usage}"));
            }

            console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(TypeConverterTask.ConversionTypeWithUsage)),
                runLines.ToArray());
        }

        [Test]
        public void DescriptionWithNoUsagesForTaskArgumentTypeConverterShowsNone()
        {
            console = GetConsoleReader<TypeConverterTask>(true,
                (cakeHost) => cakeHost
                    .RegisterTypeConverter<TypeWithUsageConverter>()
                    .RegisterTypeConverter<TypeWithoutUsageConverter>());

            Assert.IsNull(console.GetIndex(RegexProvider.GetRawUsage($"--{nameof(TypeConverterTask.OtherConversionType)}=.*"), true));
        }

        [Test]
        public void MultipleTaskConvertersWithNoSpecifiedUsageThrowsExceptionDuringDescription()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TaskConverterAttributeWithInvalidTypeThrowsExceptionDuringDescription()
        {
            throw new NotImplementedException();
        }

        private ConsoleReader GetConsoleReader<TTask>(bool includeTarget, params (string key, string? value)[] args) where TTask : IFrostingTask
        {
            var argsList = args.ToList();
            argsList.Add(("description", null));
            if (includeTarget)
            {
                var target = SprinklesDecorations.GetTaskName(typeof(TTask));
                argsList.Add(("target",target));
            }
            
            return new ConsoleReader(() =>
            {
                GetCakeHost<TTask>().Run(FormatCustomArguments(argsList.ToArray()));
            });
        }

        private ConsoleReader GetConsoleReader<TTask>(bool includeTarget, Func<CakeHost,CakeHost> configure, params (string key, string? value)[] args) where TTask : IFrostingTask
        {
            var argsList = args.ToList();
            argsList.Add(("description", null));
            if (includeTarget)
            {
                var target = SprinklesDecorations.GetTaskName(typeof(TTask));
                argsList.Add(("target", target));
            }

            return new ConsoleReader(() =>
            {
                configure(GetCakeHost<TTask>()).Run(FormatCustomArguments(argsList.ToArray()));
            });
        }
    }
}
