using Cake.Frosting;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Sprinkles.Module.TypeConversion
{
    /// <summary>
    /// Extension methods for assisting in Adding TypeConverters to the <see cref="CakeHost"/>.
    /// </summary>
    public static class TypeConverterDependencyInjectionExtensions
    {
        /// <summary>
        /// Registers a new <see cref="ITaskArgumentTypeConverter"/> for use in your tasks.
        /// </summary>
        /// <typeparam name="TTypeConverter">The <see cref="ITaskArgumentTypeConverter"/> to help convert your type.</typeparam>
        /// <param name="host">The <see cref="CakeHost"/> to register with.</param>
        /// <returns>The <see cref="CakeHost"/> with new registration added.</returns>
        public static CakeHost RegisterTypeConverter<TTypeConverter>(this CakeHost host) 
            where TTypeConverter : class, ITaskArgumentTypeConverter
        {
            return host.ConfigureServices(services => services.AddSingleton<ITaskArgumentTypeConverter, TTypeConverter>());
        }
    }
}
