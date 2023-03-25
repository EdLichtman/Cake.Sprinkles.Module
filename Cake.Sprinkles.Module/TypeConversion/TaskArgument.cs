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
    public class TaskArgument
    {
        public string Name { get; }
        public Type Type { get; }
        public PropertyInfo Property { get; }
        private readonly ICakeArguments _arguments;
        private readonly ICakeConfiguration _configuration;
        private readonly ICakeEnvironment _environment;

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
