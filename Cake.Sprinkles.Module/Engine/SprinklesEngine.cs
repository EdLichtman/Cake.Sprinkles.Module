using Cake.Core.Diagnostics;
using Cake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Sprinkles.Module.Annotations;
using Cake.Frosting;

namespace Cake.Sprinkles.Module.Engine
{
    internal class SprinklesEngine : ICakeEngine
    {
        public const string Warning_FrostingTeardownWillNotRun = "The SprinklesEngine has been overwritten by another Cake Module. Sprinkles will still Decorate your Tasks, but will not trigger your TaskTeardown or your FrostingTeardown if your arguments are invalid.";
        private readonly ICakeEngine _engine;
        private IEnumerable<IFrostingTask> _tasks;
        private readonly SprinklesTaskDescriptors _taskDescriptors;
        private readonly SprinklesDecorator _sprinklesDecorator;

        public SprinklesEngine(
            ICakeDataService data, 
            ICakeLog log, 
            IEnumerable<IFrostingTask> tasks,
            SprinklesDecorator sprinklesDecorator)
        {
            _engine = new CakeEngine(data, log);
            _tasks = tasks;
            _taskDescriptors = new SprinklesTaskDescriptors(tasks);
            _sprinklesDecorator= sprinklesDecorator;
            RegisterTaskSetupAction(DecorateSprinkles);
        }

        private void DecorateSprinkles(ITaskSetupContext context)
        {
            var target = context.Arguments.GetArgument("target");
            
            if (string.IsNullOrWhiteSpace(target))
            {
                target = "Default";
            }

            var executingTask = _taskDescriptors.Discovered.FirstOrDefault(x => x.name == target);
            if (executingTask != (default, default, default))
            {
                var task = _tasks.First(x => SprinklesDecorations.GetTaskName(x) == target);
                _sprinklesDecorator.Decorate(task);
            }
        }

        
        public CakeTaskBuilder RegisterTask(string name)
        {
            return _engine.RegisterTask(name);
        }

        public void RegisterSetupAction(Action<ISetupContext> action)
        {
            _engine.RegisterSetupAction(action);
        }

        public void RegisterSetupAction<TData>(Func<ISetupContext, TData> action) where TData : class
        {
            _engine.RegisterSetupAction(action);
        }

        public void RegisterTeardownAction(Action<ITeardownContext> action)
        {
            _engine.RegisterTeardownAction(action);
        }

        public void RegisterTeardownAction<TData>(Action<ITeardownContext, TData> action) where TData : class
        {
            _engine.RegisterTeardownAction(action);
        }

        public Task<CakeReport> RunTargetAsync(ICakeContext context, IExecutionStrategy strategy, ExecutionSettings settings)
        {
            return _engine.RunTargetAsync(context, strategy, settings);
        }

        public void RegisterTaskSetupAction(Action<ITaskSetupContext> action)
        {
            _engine.RegisterTaskSetupAction(action);
        }

        public void RegisterTaskSetupAction<TData>(Action<ITaskSetupContext, TData> action) where TData : class
        {
            _engine.RegisterTaskSetupAction(action);
        }

        public void RegisterTaskTeardownAction(Action<ITaskTeardownContext> action)
        {
            _engine.RegisterTaskTeardownAction(action);
        }

        public void RegisterTaskTeardownAction<TData>(Action<ITaskTeardownContext, TData> action) where TData : class
        {
            _engine.RegisterTaskTeardownAction(action);
        }

        public IReadOnlyList<ICakeTaskInfo> Tasks => _engine.Tasks;

        public event EventHandler<BeforeSetupEventArgs> BeforeSetup
        {
            add => _engine.BeforeSetup += value;
            remove => _engine.BeforeSetup -= value;
        }

        public event EventHandler<AfterSetupEventArgs> AfterSetup
        {
            add => _engine.AfterSetup += value;
            remove => _engine.AfterSetup -= value;
        }

        public event EventHandler<BeforeTeardownEventArgs> BeforeTeardown
        {
            add => _engine.BeforeTeardown += value;
            remove => _engine.BeforeTeardown -= value;
        }

        public event EventHandler<AfterTeardownEventArgs> AfterTeardown
        {
            add => _engine.AfterTeardown += value;
            remove => _engine.AfterTeardown -= value;
        }

        public event EventHandler<BeforeTaskSetupEventArgs> BeforeTaskSetup
        {
            add => _engine.BeforeTaskSetup += value;
            remove => _engine.BeforeTaskSetup -= value;
        }

        public event EventHandler<AfterTaskSetupEventArgs> AfterTaskSetup
        {
            add => _engine.AfterTaskSetup += value;
            remove => _engine.AfterTaskSetup -= value;
        }

        public event EventHandler<BeforeTaskTeardownEventArgs> BeforeTaskTeardown
        {
            add => _engine.BeforeTaskTeardown += value;
            remove => _engine.BeforeTaskTeardown -= value;
        }

        public event EventHandler<AfterTaskTeardownEventArgs> AfterTaskTeardown
        {
            add => _engine.AfterTaskTeardown += value;
            remove => _engine.AfterTaskTeardown -= value;
        }
    }
}
