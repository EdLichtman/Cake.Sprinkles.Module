using System.Reflection;
using System.Threading.Tasks;
using Cake.Cli;
using Cake.Core;
using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Spectre.Console;

namespace Cake.Sprinkles.Module.Engine
{
    internal class SprinklesDescriptionScriptHost : DescriptionScriptHost
    {

        private readonly IConsole _console;
        private readonly Dictionary<string, string> _descriptions;
        private readonly IDictionary<string, IFrostingTask> _tasks;

        public SprinklesDescriptionScriptHost(ICakeEngine engine, ICakeContext context, IConsole console, IEnumerable<IFrostingTask> tasks) : base(engine, context, console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _tasks = tasks.ToDictionary(SprinklesDecorations.GetTaskName);
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
            if (targets.Length == 1 && targets[0] == "Default")
            {
                targets = new String[0];
            }

            var maxTaskNameLength = 29;

            foreach (var task in _tasks.Keys)
            {
                if (task.Length > maxTaskNameLength)
                {
                    maxTaskNameLength = task.Length;
                }

                _descriptions.Add(task, SprinklesDecorations.GetTaskDescription(_tasks[task]));
            }

            maxTaskNameLength++;

            _console.WriteLine();
            _console.WriteLine(GetTaskDescriptionPrintout(maxTaskNameLength, "Task", "Description"));
            _console.WriteLine(GetSectionBreak(maxTaskNameLength));
            var taskNames = _tasks.Keys;
            if (targets.Any())
            {
                taskNames = _tasks.Keys.Intersect(targets).ToList();
            }

            foreach (var taskName in taskNames)
            {
                _console.WriteLine(GetTaskDescriptionPrintout(maxTaskNameLength, taskName, _descriptions[taskName]));
                if (targets.Any())
                {
                    _console.WriteLine();
                    PrintTaskArguments(maxTaskNameLength, targets);
                }

                var taskDescriptor = Engine.Tasks.FirstOrDefault(task => task.Name == taskName);

                if (taskDescriptor?.Dependencies.Count > 0)
                {

                    _console.WriteLine($"Task: '{taskName}' is dependent other tasks. Run --description targeting any of these for more information:", Color.Yellow3_1);
                    _console.WriteLine();
                    _console.WriteLine(GetTaskDescriptionPrintout(maxTaskNameLength, "Task", "Required"));
                    _console.WriteLine(GetSectionBreak(maxTaskNameLength));
                    foreach (var dependency in taskDescriptor.Dependencies)
                    {
                        _console.WriteLine(GetTaskDescriptionPrintout(maxTaskNameLength, dependency.Name,
                            dependency.Required.ToString()));
                    }
                }
            }

            

            if (targets.Any())
            {
                var unrecognizedTargets = targets.Except(_tasks.Keys).ToList();
                if (unrecognizedTargets.Any())
                {
                    _console.WriteLine();
                    _console.WriteLine("Unrecognized Targets");
                    _console.WriteLine(GetSectionBreak(maxTaskNameLength));
                    foreach (var target in unrecognizedTargets)
                    {
                        _console.WriteLine(target);
                    }
                }
                _console.WriteLine();

                return;
            }
            

            _console.WriteLine();
            _console.WriteLine(GetSectionBreak(maxTaskNameLength));
            _console.WriteLine("Run this command while specifying target (-t,--target) to describe the allowed arguments.");
            _console.WriteLine();
        }

        private void PrintTaskArguments(Int32 maxTaskNameLength, IList<string> targets)
        {
            _console.WriteLine("Arguments");
            _console.WriteLine(GetSectionBreak(maxTaskNameLength));
            foreach (var target in targets)
            {
                PrintTaskArguments(target);
            }
        }

        private void PrintTaskArguments(string target)
        {
            if (!_tasks.TryGetValue(target, out var task))
            {
                throw new CakeException(
                    $"Task not found: {target}.");
            }

            _console.ForegroundColor = Color.Yellow;
            _console.WriteLine($"Task: {target}");
            _console.ResetColor();
            _console.WriteLine($"Description: {SprinklesDecorations.GetTaskDescription(task)}");
            _console.WriteLine();
            var properties = task.GetType().GetProperties().Where(SprinklesDecorations.IsTaskArgument).ToList();

            if (!properties.Any())
            {
                _console.WriteLine("No Properties available.");
                _console.WriteLine();
            }

            var requiredProperties = properties.Where(SprinklesDecorations.IsRequired).ToList();
            if (requiredProperties.Any())
            {
                _console.WriteLine("The following properties are required:");
                AppendPropertiesInfo(requiredProperties);
                _console.WriteLine();
            }

            var optionalProperties = properties.Except(requiredProperties).ToList();
            if (optionalProperties.Any())
            {
                _console.WriteLine("The following properties are optional:");
                AppendPropertiesInfo(optionalProperties);
                _console.WriteLine();
            }
        }

        private void AppendPropertiesInfo(IList<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var argumentName = SprinklesDecorations.GetArgumentName(property);
                _console.ForegroundColor = Color.Yellow;
                _console.WriteLine($" * {argumentName}");
                _console.ResetColor();
                _console.WriteLine($"   * Description: {SprinklesDecorations.GetArgumentDescription(property)}");
                foreach (var usage in SprinklesDecorations.GetArgumentUsageExamples(property))
                {
                    _console.WriteLine($"   * Usage: {usage}");
                }

                var acceptsManyArguments = SprinklesDecorations.IsEnumeration(property.PropertyType);
                var typeName =
                    (acceptsManyArguments
                        ? SprinklesDecorations.GetEnumeratedType(property.PropertyType)
                        : property.PropertyType)!.Name;

                var acceptedType = $"   * Accepts: {typeName}";
                if (acceptsManyArguments)
                {
                    acceptedType += " (argument can be provided multiple times)";
                }

                _console.WriteLine(acceptedType);

                var isFlag = SprinklesDecorations.IsFlag(property);
                if (isFlag)
                {
                    _console.WriteLine($"   * Can be provided as a flag");
                }

            }
        }

        private String GetTaskDescriptionPrintout(int maxTaskNameLength, String task, String description)
        {
            return String.Format("{0,-" + maxTaskNameLength + "}{1}", task, description);
        }

        private string GetSectionBreak(int maxTaskNameLength)
        {
            return new String('=', maxTaskNameLength + 50);
        }
    }
}
