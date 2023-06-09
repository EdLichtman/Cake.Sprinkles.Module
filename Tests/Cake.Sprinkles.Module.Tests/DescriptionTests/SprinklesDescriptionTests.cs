﻿using Cake.Core;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Tests.Models.DescriptionTasks;
using Cake.Sprinkles.Module.Tests.Models.InvalidTasks;
using Cake.Sprinkles.Module.Tests.Models.StringTasks;
using Cake.Sprinkles.Module.Tests.Models.TypeConversion;
using Cake.Sprinkles.Module.Tests.Models.Validation;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Validation;
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

        [Test]
        public void DescriptionTaskWithOptionalAndRequiredDescriptionsDoesNotShowCertainDocumentation()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);

            // Does not show Task Description header, because that's only if you don't include a target, to look at all tasks.
            Assert.IsNull(console.GetIndex(RegexProvider.TaskDescriptionHeader, true));

            // Does not show a row that contains a TaskName and a Description, since that's what you see with no target
            Assert.IsNull(console.GetIndex(RegexProvider.GetNoTargetOutputForTask<NoArgumentTask>(), true));

            // Does not show the Target Task Name for any other task, for example the NoArgumentTask.
            Assert.IsNull(console.GetIndex(RegexProvider.GetTargetTaskName<NoArgumentTask>(), true));

            // Does not show the Target Task Description for any other task, for example the NoArgument Task
            Assert.IsNull(console.GetIndex(RegexProvider.GetTargetTaskDescription<NoArgumentTask>(), true));

            // Does not tell you to run it again with --target, since you have already run it with target.
            Assert.IsNull(console.GetIndex(RegexProvider.RunCommandWithTarget, true));

            // Does not tell you there are no arguments available, since there are.
            Assert.IsNull(console.GetIndex(RegexProvider.NoArgumentsAvailable, true));

            // Does not include a line with only a * on it, suggesting that no argument has been added to that line.
            Assert.IsNull(console.GetIndex(RegexProvider.GetArgument(string.Empty), true));

            // Does not include the message suggesting there is a dependency tree, because for this target, there is no dependency tree.
            Assert.IsNull(console.GetIndex(RegexProvider.DependencyTreeMessage, true));

            // Does not get a message suggesting that there is some form of validator, since there are no validated arguments on this task.
            Assert.IsNull(console.GetIndex(RegexProvider.GetValidates(".*"), true));

            //Does not get any lines that say * Description: (empty description) since initially it did.
            var emptyDescriptionsCount = console.GetIndices(RegexProvider.EmptyDescription).Count;
            Assert.That(emptyDescriptionsCount, Is.EqualTo(0), $"Expected no empty descriptions, but found: {emptyDescriptionsCount}. Also, this is the format test.{{0}}");
        }

        [Test]
        public void DescriptionShowsTaskOnLineBeforeDescription()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);
            var categorizedTaskNameIndex = console.GetIndex(RegexProvider.GetTargetTaskName<DescriptionArgumentsTask>());
            var categorizedTaskDescriptionIndex = console.GetIndex(RegexProvider.GetTargetTaskDescription<DescriptionArgumentsTask>());
            Assert.That(categorizedTaskDescriptionIndex, Is.EqualTo(categorizedTaskNameIndex + 1));
        }

        [Test]
        public void DescriptionShowsTaskDescriptionBeforeArguments()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);

            var categorizedTaskDescriptionIndex = console.GetIndex(RegexProvider.GetTargetTaskDescription<DescriptionArgumentsTask>());
            var requiredArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsRequired);

            Assert.That(requiredArgumentsHeaderIndex, Is.GreaterThan(categorizedTaskDescriptionIndex));
        }

        [Test]
        public void DescriptionWithRequiredAndOptionalValuesDisplaysRequiredValuesFirst()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);
            // Get the index of the "following arguments" headers

            // Validate the task, and that the description shows up as expected.
            var categorizedTaskNameIndex = console.GetIndex(RegexProvider.GetTargetTaskName<DescriptionArgumentsTask>());

            var requiredArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsRequired);
            var optionalArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsOptional);

            Assert.That(requiredArgumentsHeaderIndex, Is.GreaterThan(categorizedTaskNameIndex));
            Assert.That(optionalArgumentsHeaderIndex, Is.GreaterThan(requiredArgumentsHeaderIndex));
        }


        [Test(Description =
            "Tests should normally do only one thing, but the sheer definition of such a test, to make sure documentation " +
            "is stable, requires we break this rule. This is a bad example of a test, but a necessary one.")]
        public void DescriptionWithDescriptiveTargetBringsBackOnlyDescriptiveTarget()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);
            // Get the index of the "following arguments" headers

            var optionalArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsOptional);


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


        }

        [Test]
        public void DescriptionOfRequiredArgumentIsInBetweenRequiredAndOptionalArguments()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);
            var requiredArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsRequired);
            var optionalArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsOptional);

            var requiredValue = nameof(DescriptionArgumentsTask.RequiredValue);

            console.ConfirmLineBetween(
                requiredArgumentsHeaderIndex,
                RegexProvider.GetArgument(requiredValue),
                optionalArgumentsHeaderIndex);
        }

        [Test]
        public void DescriptionOfOptionalArgumentIsInOptionalArguments()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);
            var optionalArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsOptional);

            var enumValue = nameof(DescriptionArgumentsTask.DescriptionEnum);

            console.ConfirmLineBetween(
                optionalArgumentsHeaderIndex,
                RegexProvider.GetArgument(enumValue));
        }

        [Test] 
        public void DescriptionOfEnumDisplaysAcceptedValues()
        {
            console = GetConsoleReader<DescriptionArgumentsTask>(true);
            var enumValue = nameof(DescriptionArgumentsTask.DescriptionEnum);
            console.ConfirmRunContains(
                RegexProvider.GetArgument(enumValue),
                RegexProvider.GetAcceptsType<DescriptionArgumentsTask>(enumValue),
                new Regex("^ *\\* " + DescriptionEnum.EnumOne + " *$"),
                new Regex("^ *\\* " + DescriptionEnum.FightingMongooses + " *$"));
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
                    .RegisterTypeConverter<TypeWithoutUsageConverter>()
                    .RegisterTypeConverter<TypeWithUsageListConverter>());

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
                    .RegisterTypeConverter<TypeWithoutUsageConverter>()
                    .RegisterTypeConverter<TypeWithUsageListConverter>());

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
                    .RegisterTypeConverter<TypeWithoutUsageConverter>()
                    .RegisterTypeConverter<TypeWithUsageListConverter>());

            Assert.IsNull(console.GetIndex(RegexProvider.GetRawUsage($"--{nameof(TypeConverterTask.OtherConversionType)}=.*"), true));
        }

        [Test]
        public void MultipleTaskConvertersWithNoneSpecifiedThrowsExceptionDuringDescription()
        {
            console = GetConsoleReader<TypeConverterInvalidTask>(true,
                host => host
                    .RegisterTypeConverter<TypeWithUsageConverter>()
                    .RegisterTypeConverter<TypeWithUsageOtherConverter>());

            var compileErrorOccurredIndex = console.GetIndex(RegexProvider.CompileErrorOccurred);
            var noConvertersSpecifiedRegex = new Regex("^" + SprinklesValidator.Message_ArgumentConverterMultipleMustHaveAnnotation + ".*");

            console.ConfirmLineBetween(
                compileErrorOccurredIndex,
                noConvertersSpecifiedRegex);
        }

        [Test]
        public void TaskConverterAttributeWithInvalidTypeThrowsExceptionDuringDescription()
        {
            console = GetConsoleReader<TypeConverterInvalidTask>(true,
                host => host
                    .RegisterTypeConverter<TypeWithUsageConverter>()
                    .RegisterTypeConverter<TypeWithUsageOtherConverter>());

            var compileErrorOccurredIndex = console.GetIndex(RegexProvider.CompileErrorOccurred);
            var notValidTypeRegex = new Regex("^" + SprinklesValidator.Message_ArgumentConverterNotValid + ".*");

            console.ConfirmLineBetween(
                compileErrorOccurredIndex,
                notValidTypeRegex);
        }

        [Test]
        public void SystemComponentModelConverterAttributeListsExamplesIfApplicable()
        {
            console = GetConsoleReader<SystemComponentModelTypeConverterTask>(true);

            console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(SystemComponentModelTypeConverterTask.MyCustomType)),
                RegexProvider.GetAcceptsType<MyCustomType>(),
                RegexProvider.GetRawUsage($"--{nameof(SystemComponentModelTypeConverterTask.MyCustomType)}={MyCustomType.CustomExample}"));
        }

        [Test]
        public void SystemComponentModelConverterAttributeDoesNotListExamplesIfNotApplicable()
        {
            console = GetConsoleReader<SystemComponentModelTypeConverterTask>(true);

            console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(SystemComponentModelTypeConverterTask.MyOtherCustomType)),
                RegexProvider.GetAcceptsType<MyOtherCustomType>());
        }

        [Test(Description = $"This pairs with {nameof(CanGetTaskDescriptionForDefaultTask)} to prove that all tasks can be named when no task is specified, and that it does not cause Default task to be described.")]
        public void CanGetAllTaskDescriptionsWhenSpecifyingNoTarget()
        {
            console = GetConsoleReader<DefaultTask>(false);

            Assert.IsNull(console.GetIndex(RegexProvider.DependencyTreeMessage, true));

            // Not using Assert.Multiple because when we aggregate it, and there's an error, we can't see the stack trace and know where the error is.
            var headerTitleIndex = console.GetIndex(RegexProvider.TaskDescriptionHeader);
            var headerSpacerLines = console.GetIndices(RegexProvider.HeaderSeparater);
            Assert.That(headerSpacerLines, Does.Contain(headerTitleIndex + 1));

            var defaultTaskIndex = console.GetIndex(RegexProvider.GetNoTargetOutputForTask<DefaultTask>());
            Assert.That(defaultTaskIndex, Is.GreaterThan(headerTitleIndex));

            var categorizedTaskIndex = console.GetIndex(RegexProvider.GetNoTargetOutputForTask<NoArgumentTask>());
            Assert.That(categorizedTaskIndex, Is.GreaterThan(headerTitleIndex));

            var uncategorizedTaskIndex = console.GetIndex(RegexProvider.GetNoTargetOutputForTask<DescriptionArgumentsTask>());
            Assert.That(uncategorizedTaskIndex, Is.GreaterThan(headerTitleIndex));

            var runThisCommandFurtherIndex = console.GetIndex(RegexProvider.RunCommandWithTarget);

            Assert.That(headerSpacerLines, Does.Contain(runThisCommandFurtherIndex - 1));
        }
        [Test(Description = "When putting this together I noticed that the target getting passed into the CakeHost was 'Default', so I blatantly ignored the target. This causes the Default task to not get described.")]
        public void CanGetTaskDescriptionForDefaultTask()
        {
            console = GetConsoleReader<DefaultTask>(true);
            // Get the index of the "following arguments" headers

            // Validate the task, and that the description shows up as expected.
            var defaultTaskNameIndex = console.GetIndex(RegexProvider.GetTargetTaskName<DefaultTask>());
            var optionalArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsOptional);
            Assert.That(optionalArgumentsHeaderIndex, Is.GreaterThan(defaultTaskNameIndex));

            var run = console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(DefaultTask.DefaultDescribedValue)),
                RegexProvider.GetArgumentDescription<DefaultTask>(nameof(DefaultTask.DefaultDescribedValue)),
                RegexProvider.GetAcceptsType<string>());

            Assert.That(run.Start.Value, Is.GreaterThan(optionalArgumentsHeaderIndex));
        }

        [Test]
        public void CanRequireCustomConversionType()
        {
            console = GetConsoleReader<CustomTypeTask>(
                true,
                host => host.RegisterTypeConverter<TypeWithUsageConverter>());

            Assert.IsNull(console.GetIndex(RegexProvider.FollowingArgumentsOptional, true));

            var requiredArgumentsHeaderIndex = console.GetIndex(RegexProvider.FollowingArgumentsRequired);
            
            console.ConfirmAllLinesBetween(
                requiredArgumentsHeaderIndex,
                RegexProvider.GetArgument(nameof(CustomTypeTask.CustomType)));
        }

        [Test]
        public void CanDisplayUsageExamplesForCustomConversionType()
        {
            console = GetConsoleReader<CustomTypeTask>(
                true,
                host => host.RegisterTypeConverter<TypeWithUsageConverter>());

            var runRegex = 
                new TypeWithUsageConverter()
                .GetExampleInputValues()
                .Select(x => RegexProvider.GetRawUsage($"--CustomType={x}"))
                .ToList();

            runRegex.Add(RegexProvider.GetAcceptsType<TypeWithUsage>());
            console.ConfirmRunContains(
                RegexProvider.GetArgument(nameof(CustomTypeTask.CustomType)),
                runRegex.ToArray());
        }

        [Test]
        public void NotifiesYouBeforeDescriptionIfYouForgotToRegisterTaskArgument()
        {
            console = GetConsoleReader<CustomTypeTask>(
                true);

            Assert.That(console.GetIndex(RegexProvider.CompileErrorOccurred, true), Is.Not.Null);
            Assert.That(console.GetIndex(RegexProvider.ForgotToRegisterTypeConverter, true), Is.Not.Null);
        }

        private ConsoleReader GetConsoleReader<TTask>(bool includeTarget, params (string key, string? value)[] args) where TTask : IFrostingTask
        {
            var argsList = args.ToList();
            argsList.Add(("description", null));
            if (includeTarget)
            {
                var target = SprinklesDecorations.GetTaskName(typeof(TTask));
                argsList.Add(("target",target));
                argsList.Add(("arguments", null));
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
                argsList.Add(("arguments", null));
            }

            return new ConsoleReader(() =>
            {
                configure(GetCakeHost<TTask>()).Run(FormatCustomArguments(argsList.ToArray()));
            });
        }
    }
}
