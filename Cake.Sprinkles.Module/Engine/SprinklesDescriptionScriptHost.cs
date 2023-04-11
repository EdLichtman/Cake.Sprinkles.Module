using System.Reflection;
using Cake.Cli;
using Cake.Common;
using Cake.Core;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.TypeConversion;
using Cake.Sprinkles.Module.Validation;
using Cake.Sprinkles.Module.Validation.Exceptions;
using NuGet.Packaging;
using Spectre.Console;

namespace Cake.Sprinkles.Module.Engine
{
    internal class SprinklesDescriptionScriptHost : DescriptionScriptHost
    {
        private const string Message_FixErrorsFirst = "Error(s) occurred during compilation. Please fix the task before you can run this tool.";
        private const string Message_ThereAreDependencies = "This task has one or more dependencies. Run the dependency tree tool (--tree) to discover those dependencies, and then run the description tool (--description) while specifying target (-t,--target) as one of those dependencies to describe the allowed arguments for that dependency.";
        private const string Message_RunCommandWithTargetForArguments = "Run this command while specifying target (-t [TARGET] | --target [TARGET]), and requesting arguments (--arguments) to describe the allowed arguments.";
        private readonly IEnumerable<ITaskArgumentTypeConverter> _taskArgumentTypeConverters;
        private readonly IConsole _console;
        private readonly SprinklesTaskDescriptors _taskDescriptors;
        private readonly SprinklesValidator _validator;
        private readonly SprinklesArgumentsProvider _argumentsProvider;
        private readonly int _maxTaskNameLength;
        private readonly bool _printArguments;

        public SprinklesDescriptionScriptHost(
            ICakeEngine engine, 
            ICakeContext context, 
            IConsole console, 
            SprinklesTaskDescriptors tasks, 
            IEnumerable<ITaskArgumentTypeConverter> taskArgumentTypeConverters,
            SprinklesValidator validator,
            SprinklesArgumentsProvider argumentsProvider) 
            : base(engine, context, console)
        {
            _taskArgumentTypeConverters = taskArgumentTypeConverters;
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _taskDescriptors = tasks;
            _validator = validator;
            _argumentsProvider = argumentsProvider;
            _maxTaskNameLength = GetMaxTaskNameLength();
            _printArguments = context.HasArgument("arguments");
        }

        public override Task<CakeReport> RunTargetAsync(string target)
        {
            PrintTaskDescriptions(target);

            return System.Threading.Tasks.Task.FromResult<CakeReport>(null!);
        }

        //private void 

        public override Task<CakeReport> RunTargetsAsync(IEnumerable<string> targets)
        {
            PrintTaskDescriptions(targets.ToArray());

            return System.Threading.Tasks.Task.FromResult<CakeReport>(null!);
        }


        private void PrintTaskDescriptions(params string[] targets)
        {
            if (!_printArguments)
            {
                PrintAllDescriptions();
            } 
            else
            {
                var unrecognizedTargets = targets.Except(_taskDescriptors.Discovered.Select(x => x.name)).ToList();
                if (unrecognizedTargets.Any())
                {
                    _console.WriteLine();
                    _console.WriteLine("Unrecognized Targets");
                    _console.WriteLine(GetSectionBreak());
                    foreach (var target in unrecognizedTargets)
                    {
                        _console.WriteLine(target);
                    }
                } 
                else
                {
                    PrintTaskArguments(targets);
                }
                _console.WriteLine();
            }
        }

        private void PrintAllDescriptions()
        {
            _console.WriteLine();
            _console.WriteLine(GetTaskDescriptionPrintout("Task", "Description"));
            _console.WriteLine(GetSectionBreak());
            _console.WriteLine();
            foreach(var descriptor in _taskDescriptors.Discovered)
            {
                _console.WriteLine(GetTaskDescriptionPrintout(descriptor.name, descriptor.description));
            }

            _console.WriteLine(GetSectionBreak());
            _console.WriteLine(Message_RunCommandWithTargetForArguments);
            _console.WriteLine();
        }

        private void PrintTaskArguments(IList<string> targets)
        {
            foreach (var target in targets)
            {
                PrintTaskArguments(target);
            }
        }

        private void PrintTaskArguments(string target)
        {
            var descriptor = _taskDescriptors.Discovered.First(x => x.name == target);

            _console.ForegroundColor = Color.Yellow;
            _console.WriteLine($"Task: {target}");
            _console.ResetColor();
            _console.WriteLine($"Description: {descriptor.description}");
            _console.WriteLine();

            var exceptions = new List<SprinklesException>();
            var argumentsOfTask = _argumentsProvider.GetAllArguments(descriptor.task, exceptions);
            
            var properties = argumentsOfTask
                .Select(x => x.property)
                .ToList();

            if (!properties.Any())
            {
                _console.WriteLine("No Arguments available.");
                _console.WriteLine();
            }

            if (!ValidatePropertiesInfo(properties, exceptions))
            {
                return;
            }

            var requiredProperties = properties.Where(SprinklesDecorations.IsRequired).ToList();
            if (requiredProperties.Any())
            {
                _console.WriteLine("The following arguments are required:");
                AppendPropertiesInfo(requiredProperties);
            }

            var optionalProperties = properties.Except(requiredProperties).ToList();
            if (optionalProperties.Any())
            {
                _console.WriteLine("The following arguments are optional:");
                AppendPropertiesInfo(optionalProperties);
            }

            var taskDescriptor = Engine.Tasks.FirstOrDefault(task => task.Name == target);

            if (taskDescriptor?.Dependencies.Count > 0)
            {
                _console.WriteLine();
                _console.WriteLine(Message_ThereAreDependencies);
            }
        }

        private bool ValidatePropertiesInfo(IList<PropertyInfo> properties, IList<SprinklesException> exceptions)
        {
            var propertiesThatCanBeAllowedNames =
                properties
                    .Where(x => !SprinklesDecorations.IsExternalTaskArguments(x))
                    .ToList();

            var propertyNames =
                propertiesThatCanBeAllowedNames
                    .Select(SprinklesDecorations.GetArgumentName)
                    .ToList();

            foreach (var property in properties)
            {
                var namespaceClassQualifiedPropertyName = SprinklesDecorations.GetNamespaceClassQualifiedPropertyName(property);
                if (exceptions.Any(x => x.NamespaceClassQualifiedPropertyName == namespaceClassQualifiedPropertyName))
                {
                    continue;
                }

                try
                {
                    SprinklesValidator.ValidateDuplicateBuildClassProperty(property, propertyNames);
                    _validator.ValidateBuildClassProperty(property);
                }
                catch (SprinklesException ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
            {
                _console.WriteLine(Message_FixErrorsFirst);
                foreach (var exception in exceptions)
                {
                    _console.WriteLine(exception.Message);
                }
            }

            return !exceptions.Any();
        }

        private void AppendPropertiesInfo(IList<PropertyInfo> properties)
        {
            foreach(var property in properties)
            {
                AppendPropertyInfo(property);
            }   
        }

        private void AppendPropertyInfo(PropertyInfo property)
        {
            var argumentName = SprinklesDecorations.GetArgumentName(property);
            _console.ForegroundColor = Color.Yellow;
            _console.WriteLine($" * {argumentName}");
            _console.ResetColor();
            var description = SprinklesDecorations.GetArgumentDescription(property);
            if (!string.IsNullOrWhiteSpace(description))
            {
                _console.WriteLine($"   * Description: {description}");
            }

            var acceptsManyArguments = SprinklesDecorations.IsEnumeration(property.PropertyType);
            var type =
                (acceptsManyArguments
                    ? SprinklesDecorations.GetEnumeratedType(property.PropertyType)
                    : property.PropertyType);

            var usages = SprinklesDecorations.GetArgumentExampleValues(property);
            var converter = _taskArgumentTypeConverters.FirstOrDefault(x => x.ConversionType == type);
            if (converter != null)
            {
                usages.AddRange(converter.GetExampleInputValues());
            }

            usages = usages.Select(usage => $"--{argumentName}={usage}").ToList();

            foreach (var usage in usages)
            {
                _console.WriteLine($"   * Usage: {usage}");
            }

            var typeName = type!.Name;
            var acceptedType = $"   * Accepts: {typeName}";
            if (acceptsManyArguments)
            {
                var delimiter = SprinklesDecorations.GetArgumentEnumerationDelimiterName(property);
                var hasDelimiter = !string.IsNullOrWhiteSpace(delimiter);

                if (hasDelimiter)
                {
                    acceptedType += $"{Environment.NewLine}   * Parses single argument using Delimiter: {delimiter}";
                }
                else
                {
                    acceptedType += " (argument can be provided multiple times)";
                }
            }

            _console.WriteLine(acceptedType);

            if (type.IsEnum)
            {
                var enumValues = Enum.GetValues(type);
                foreach(var enumValue in enumValues)
                {
                    _console.WriteLine($"     * {enumValue}");
                }
            }

            var validations = SprinklesDecorations.GetArgumentValidations(property);
            foreach(var validation in validations)
            {
                _console.WriteLine($"     * Validation: {validation.DescriptionOfValidation}");
            }

            var isFlag = SprinklesDecorations.IsFlag(property);
            if (isFlag)
            {
                _console.WriteLine($"   * Can be provided as a flag");
            }

            _console.WriteLine();
        }
        private String GetTaskDescriptionPrintout(String task, String description)
        {
            return String.Format("{0,-" + _maxTaskNameLength + "}{1}", task, description);
        }

        private string GetSectionBreak()
        {
            return new String('=', _maxTaskNameLength + 50);
        }

        private int GetMaxTaskNameLength()
        {
            var maxTaskNameLength = 29;

            foreach (var task in _taskDescriptors.Discovered)
            {
                if (task.name.Length > maxTaskNameLength)
                {
                    maxTaskNameLength = task.name.Length;
                }
            }

            maxTaskNameLength++;
            return maxTaskNameLength;
        }
    }
}
