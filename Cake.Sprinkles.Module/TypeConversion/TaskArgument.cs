using Cake.Core;
using Cake.Core.Configuration;
using Cake.Sprinkles.Module.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.TypeConversion
{
    /// <summary>
    /// A means of obtaining the Task Argument Value from the CLI arguments, the cake.config file, and the environment variables.
    /// </summary>
    public class TaskArgument
    {
        /// <summary>
        /// Gets the Name of the Task Argument.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Property <see cref="System.Type"/> of the Task Argument
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the Task Argument
        /// </summary>
        public PropertyInfo Property { get; }
        private readonly ICakeArguments _arguments;
        private readonly ICakeConfiguration _configuration;
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskArgument"/> class.
        /// </summary>
        /// <param name="arguments">The CLI arguments.</param>
        /// <param name="configuration">The cake.config file configuration.</param>
        /// <param name="environment">The environment variables.</param>
        /// <param name="property">The <see cref="PropertyInfo"/> for the Task Argument.</param>
        public TaskArgument(
            ICakeArguments arguments, 
            ICakeConfiguration configuration, 
            ICakeEnvironment environment, 
            PropertyInfo property)
        {
            _arguments = arguments;
            _configuration = configuration;
            _environment = environment;
            Property = property;
            Name = SprinklesDecorations.GetArgumentName(property);
            // No getting of Enumeration types because this is a flat conversion.
            Type = property.PropertyType;
        }

        /// <summary>
        /// Gets the value as a single nullable string.
        /// </summary>
        /// <returns>The value of the Task Argument.</returns>
        public string? GetValue()
        {
            var isFlag = SprinklesDecorations.IsFlag(Property);
            var value = _arguments.GetArgument(Name);
            if (value == null)
            {
                value = _configuration.GetValue(Name);
                if (value == null)
                {
                    value = _environment.GetEnvironmentVariable(Name);
                }
            }

            if (string.IsNullOrWhiteSpace(value?.ToString()) && isFlag && Property.PropertyType == typeof(bool))
            {
                value = _arguments.HasArgument(Name).ToString();
            }

            return value?.ToString();
        }

        /// <summary>
        /// Gets the value as a list of strings.
        /// </summary>
        /// <returns>The value of the Task Argument.</returns>
        public IList<string> GetValues()
        {
            var list = new List<string>();
            list.AddRange(_arguments.GetArguments(Name).ToList());

            if (!list.Any())
            {
                var configurationValue = _configuration.GetValue(Name);
                if (!string.IsNullOrWhiteSpace(configurationValue))
                {
                    list.Add(configurationValue);
                }

                if (!list.Any())
                {
                    var environmentValue = _environment.GetEnvironmentVariable(Name);
                    if (!string.IsNullOrWhiteSpace(environmentValue))
                    {
                        list.Add(environmentValue);
                    }
                }
            }

            return list;
        }
    }
}
