using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.DescriptionTests
{
    internal static class RegexProvider
    {
        public static Regex HeaderSeparater => new Regex("^=*$");
        public static Regex TaskDescriptionHeader => new Regex("^Task *Description *$");
        public static Regex DependencyTreeMessage => new Regex("^This task has one or more dependencies\\. Run the dependency tree tool \\(--tree\\) to discover those dependencies, and then run the description tool \\(--description\\) while specifying target \\(-t,--target\\) as one of those dependencies to describe the allowed arguments for that dependency\\.$");
        public static Regex RunCommandWithTarget => new Regex("^Run this command while specifying target \\(-t \\[TARGET\\] \\| --target \\[TARGET\\]\\), and requesting arguments \\(--arguments\\) to describe the allowed arguments.$");
        public static Regex NoArgumentsAvailable => new Regex("^No Arguments available.$");
        public static Regex CanBeFlag => new Regex("^ *\\* Can be provided as a flag$");
        /// <summary>
        /// A regex representing an empty description. Sprinkles should never output an empty description.
        /// </summary>
        public static Regex EmptyDescription => new Regex("^ *\\* Description: *$");
        public static Regex EmptyLine => new Regex("^ *$");

        public static Regex AcceptsArgumentType => new Regex("^ *\\* Accepts: (.*)$");
        public static Regex FollowingArgumentsRequired => new Regex("^The following arguments are required: *$");
        public static Regex FollowingArgumentsOptional => new Regex("^The following arguments are optional: *$");
        public static Regex CompileErrorOccurred => new Regex("^Error\\(s\\) occurred during compilation\\. Please fix the task before you can run this tool\\.$");
        public static Regex ForgotToRegisterTypeConverter => new Regex(SprinklesValidator.Message_BeSureToAddTypeConverter);
        public static Regex GetNoTargetOutputForTask<TTask>() where TTask : IFrostingTask
        {
            var taskName = SprinklesDecorations.GetTaskName(typeof(TTask));
            var taskDescription = SprinklesDecorations.GetTaskDescription(typeof(TTask));
            return new Regex($"^{taskName} *{taskDescription} *$");
        }

        public static Regex GetTargetTaskName<TTask>() where TTask : IFrostingTask
        {
            return new Regex($"^Task: {SprinklesDecorations.GetTaskName(typeof(TTask))} *$");
        }
        public static Regex GetTargetTaskDescription<TTask>() 
        {
            return new Regex($"^Description: {SprinklesDecorations.GetTaskDescription(typeof(TTask))} *$");
        }
        public static Regex GetArgument(string propertyName)
        {
            return new Regex($"^ *\\* {propertyName} *$");
        }

        public static Regex? GetArgumentDescription<TTask>(string propertyName) 
        {
            var propertyInfo = GetProperty<TTask>(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            return new Regex($"^ *\\* Description: {SprinklesDecorations.GetArgumentDescription(propertyInfo)} *$");
        }

        public static Regex? GetUsage<TTask>(string propertyName)
        {
            var propertyInfo = GetProperty<TTask>(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            var name = SprinklesDecorations.GetArgumentName(propertyInfo);
            var usage = SprinklesDecorations.GetArgumentExampleValues(propertyInfo).FirstOrDefault();
            if (usage == null)
            {
                return null;
            }

            return new Regex($"^ *\\* Usage: --{name}={usage}");
        }

        public static Regex GetRawUsage(string rawUsage)
        {
            return new Regex($"^ *\\* Usage: {rawUsage}");
        }

        public static Regex GetAcceptsType<TType>(bool hasEnumerationDelimiter = false)
        {
            var type = typeof(TType);

            var isEnumeration = SprinklesDecorations.IsEnumeration(type);

            if (isEnumeration)
            {
                var enumeratedType = SprinklesDecorations.GetEnumeratedType(type);

                if (hasEnumerationDelimiter)
                {
                    return new Regex($"^ *\\* Accepts: {enumeratedType!.Name} *$");
                }

                return new Regex($"^ *\\* Accepts: {enumeratedType!.Name} \\(argument can be provided multiple times\\)$");
            }

            return new Regex($"^ *\\* Accepts: {type.Name} *$");
        }

        public static Regex? GetAcceptsType<TTask>(string propertyName) 
        {
            var propertyInfo = GetProperty<TTask>(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            var isEnumeration = SprinklesDecorations.IsEnumeration(propertyInfo.PropertyType);

            if (isEnumeration)
            {
                var enumeratedType = SprinklesDecorations.GetEnumeratedType(propertyInfo.PropertyType);
                
                if (SprinklesDecorations.HasArgumentEnumerationDelimiter(propertyInfo))
                {
                    return new Regex($"^ *\\* Accepts: {enumeratedType!.Name} *$");
                }

                return new Regex($"^ *\\* Accepts: {enumeratedType!.Name} \\(argument can be provided multiple times\\)$");
            }
            
            return new Regex($"^ *\\* Accepts: {propertyInfo.PropertyType.Name} *$");
        }

        public static Regex? GetRecognizesDelimiter<TTask>(string propertyName) where TTask : IFrostingTask
        {
            var propertyInfo = GetProperty<TTask>(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            return new Regex($"^ *\\* Parses single argument using Delimiter: ({SprinklesDecorations.GetArgumentEnumerationDelimiterName(propertyInfo)})$");
        }

        public static Regex GetValidates(string validation)
        {
            return new Regex($"^ *\\* Validation: {validation}$");
        }

        private static PropertyInfo? GetProperty<TTask>(string propertyName)
        {
            var propertyInfo = typeof(TTask).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                Assert.Fail($"Could not find Property '{propertyName}' on Type '{typeof(TTask).Name}'");
                return null;
            }

            return propertyInfo;
        }

    }
}
