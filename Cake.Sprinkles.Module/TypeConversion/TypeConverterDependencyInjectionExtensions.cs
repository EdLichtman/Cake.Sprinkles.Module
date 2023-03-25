using Cake.Frosting;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Sprinkles.Module.TypeConversion
{
    public static class TypeConverterDependencyInjectionExtensions
    {
        public static CakeHost RegisterTypeConverter<TTypeConverter>(this CakeHost host) 
            where TTypeConverter : class, ITaskArgumentTypeConverter
        {
            return host.ConfigureServices(services => services.AddSingleton<ITaskArgumentTypeConverter, TTypeConverter>());
        }
    }
}
