using Cake.Frosting;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Tests.Models;
using Cake.Sprinkles.Module.Validation.Exceptions;
using System;

namespace Cake.Sprinkles.Module.Tests
{
    internal class SprinklesTestBase
    {
        protected int RunCakeHost<TFrostingTask>(IEnumerable<(string, string)> args) where TFrostingTask : IFrostingTask
        {
            var argsList = args.ToList();
            argsList.Add(("target", SprinklesDecorations.GetTaskName(typeof(TFrostingTask))));
            var taskAssembly = typeof(TFrostingTask).Assembly;
            return new CakeHost()
                .UseContext<SprinklesTestContext<TFrostingTask>>()
                .UseTeardown<SprinklesTestTeardown>()
                .UseModule<SprinklesDescriptionModule>()
                .AddAssembly(taskAssembly)
                .Run(FormatCustomArguments(argsList.Select(x => (x.Item1, (string?)x.Item2)).ToArray()));
        }

        protected int RunCakeHost<TFrostingTask>(params (string, string?)[] args) where TFrostingTask : IFrostingTask
        {
            var argsList = args.ToList();
            argsList.Add(("target", SprinklesDecorations.GetTaskName(typeof(TFrostingTask))));
            var taskAssembly = typeof(TFrostingTask).Assembly;
            return new CakeHost()
                .UseContext<SprinklesTestContext<TFrostingTask>>()
                .UseTeardown<SprinklesTestTeardown>()
                .UseModule<SprinklesDescriptionModule>()
                .AddAssembly(taskAssembly)
                .Run(FormatCustomArguments(argsList.ToArray()));
        }

        protected CakeHost GetCakeHost<TFrostingTask>() where TFrostingTask : IFrostingTask
        {
            var taskAssembly = typeof(TFrostingTask).Assembly;
            return new CakeHost()
                .UseContext<SprinklesTestContext<TFrostingTask>>()
                .UseTeardown<SprinklesTestTeardown>()
                .UseModule<SprinklesDescriptionModule>()
                .AddAssembly(taskAssembly);
        }

        protected TTask? GetContext<TTask>() where TTask : IFrostingTask
        {
            var context = (SprinklesTestContextProvider.Context as SprinklesTestContext<TTask>);
            if (context == null)
            {
                return default;
            }
            return context.Task;
        }

        protected SprinklesException? FilterSprinklesExceptionForProperty<TType>(IList<SprinklesException>? exceptions, string propertyName)
        {
            var property = typeof(TType).GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }

            var namespaceClassQualifiedPropertyName = SprinklesDecorations.GetNamespaceClassQualifiedPropertyName(property);
            return exceptions?.FirstOrDefault(x => x.NamespaceClassQualifiedPropertyName == namespaceClassQualifiedPropertyName);
        }

        protected SprinklesException? GetSprinklesExceptionForProperty<TType>(string propertyName)
        {
            return FilterSprinklesExceptionForProperty<TType>(GetSprinklesExceptions(), propertyName);
        }

        protected IList<SprinklesException>? GetSprinklesExceptions()
        {
            var thrownException = (SprinklesTestContextProvider.ThrownException as AggregateException);
            var innerExceptions = thrownException?
                .InnerExceptions?
                .Select(x => x as SprinklesException)
                .Except(new SprinklesException?[] { null })
                // required for nullability, we make sure we're not including nulls above.
                .Cast<SprinklesException>();

            return innerExceptions?.ToList();
        }

        protected (string key, string? value)[] PrepareArguments(IEnumerable<(string, string)> arguments)
        {
            return arguments.Select(x => (x.Item1, (string?)x.Item2)).ToArray();
        }

        protected string[] FormatCustomArguments(string target, params (string key, string? value)[] arguments)
        {
            var argumentsWithTarget = arguments.ToList();
            argumentsWithTarget.Add(("target", target));

            return FormatCustomArgumentsEnumerable(argumentsWithTarget.ToArray()).ToArray();
        }

        protected string[] FormatCustomArguments(params (string key, string? value)[] arguments)
        {
            return FormatCustomArgumentsEnumerable(arguments).ToArray();
        }

        private IEnumerable<string> FormatCustomArgumentsEnumerable(params (string key, string? value)[] arguments)
        {
            foreach (var argument in arguments)
            {
                if (string.IsNullOrWhiteSpace(argument.value))
                {
                    yield return string.Format("--{0}", argument.key);
                }
                else
                {
                    yield return string.Format("--{0}={1}", argument.key, argument.value);
                }

            }
        }
    }
}
