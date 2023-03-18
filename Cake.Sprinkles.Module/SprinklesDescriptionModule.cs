using Cake.Cli;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Composition;
using Cake.Frosting;
using Cake.Sprinkles.Module;
using Cake.Sprinkles.Module.Engine;

[assembly: CakeModule(typeof(SprinklesDescriptionModule))]

namespace Cake.Sprinkles.Module
{
    public class SprinklesDescriptionModule : ICakeModule
    {
        public void Register(ICakeContainerRegistrar registrar)
        {
            registrar.RegisterType<SprinklesDescriptionScriptHost>().As<DescriptionScriptHost>().Singleton();
            // note -- I don't like this because it forces the user to use MY one and only TaskSetup. I need approval from the Cake Frosting Team to add this internally
            // so it doesn't take up any Task Setups
            registrar.RegisterType<SprinklesDescriptionTaskSetUp>().As<IFrostingTaskSetup>();
        }
    }
}
