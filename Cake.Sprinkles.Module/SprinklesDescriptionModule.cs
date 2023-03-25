using Cake.Cli;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Composition;
using Cake.Frosting;
using Cake.Sprinkles.Module;
using Cake.Sprinkles.Module.Annotations;
using Cake.Sprinkles.Module.Engine;
using Cake.Sprinkles.Module.Validation;

[assembly: CakeModule(typeof(SprinklesDescriptionModule))]

namespace Cake.Sprinkles.Module
{
    public class SprinklesDescriptionModule : ICakeModule
    {
        public void Register(ICakeContainerRegistrar registrar)
        {
            registrar.RegisterType<SprinklesDescriptionScriptHost>().As<DescriptionScriptHost>().Singleton();
            registrar.RegisterType<SprinklesDecorator>().Singleton();
            registrar.RegisterType<SprinklesTaskDescriptors>().Singleton();
            registrar.RegisterType<SprinklesValidator>().Singleton();
            registrar.RegisterType<SprinklesArgumentsProvider>().Singleton();


            // First, overwrite the engine to decorate the sprinkles during the request pipeline
            registrar.RegisterType<SprinklesEngine>().As<ICakeEngine>().Singleton();

            // Then, if someone has included another module that overwrites the SprinklesEngine,
            // the buildScriptHost override has a backup, albeit less preferable method for injecting the arguments.
            registrar.RegisterType<SprinklesBuildScriptHost>().As<BuildScriptHost>().Singleton();
            registrar.RegisterType<SprinklesBuildScriptHost>().As<BuildScriptHost<ICakeContext>>().Singleton();
            registrar.RegisterType<SprinklesBuildScriptHost>().As<BuildScriptHost<IFrostingContext>>().Singleton();
            
        }
    }
}
