using System.Reflection;
using Cake.Frosting;
using Cake.Sprinkles.Module;
using Cake.Sprinkles.Module.Tests.Models;

namespace Cake.Sprinkles.Module.Tests
{
    internal class SprinklesTestBase
    {
        protected CakeHost GetCakeHost<TFrostingTask>() where TFrostingTask : IFrostingTask
        {
            var taskAssembly = typeof(TFrostingTask).Assembly;
            return new CakeHost()
                .UseContext<SprinklesTestContext<TFrostingTask>>()
                .UseTeardown<SprinklesTestTeardown>()
                .UseModule<SprinklesDescriptionModule>()
                .AddAssembly(taskAssembly);
        }

        protected IList<String> GetAllPropertiesAsNumbers(bool onlyRequired = false)
        {
            return GetAllPropertiesWithFormatting("={0}", onlyRequired).ToList();
        }

        protected IList<String> GetAllPropertiesAsStrings(bool onlyRequired = false)
        {
            return GetAllPropertiesWithFormatting("=test_{0}", onlyRequired).ToList();
        }

        protected IList<String> GetAllPropertiesAsBooleans(bool onlyRequired = false)
        {
            return GetAllPropertiesWithFormatting("=true", onlyRequired).ToList();
        }

        private IEnumerable<String> GetAllPropertiesWithFormatting(String formatString = "", bool onlyRequired = false)
        {
            var listWithDuplicates = new List<String>();
            var propertyKeys = onlyRequired ? PropertyKeys.Where(x => x.StartsWith("required")) : PropertyKeys;
            foreach (var key in propertyKeys)
            {
                listWithDuplicates.Add(key);
                listWithDuplicates.Add(key);
            }

            for (var i = 0; i < listWithDuplicates.Count; i++)
            {
                var key = listWithDuplicates[i];
                if (formatString.Contains("{0}"))
                    yield return $"--{key}" + String.Format(formatString, i);
                else
                    yield return $"--{key}" + formatString;
            }
        }
        protected IList<String> PropertyKeys = new List<String>()
        {
            "required_single",
            "required_list",
            "required_hashset",
            "optional_single",
            "optional_list",
            "optional_hashset"
        };
    }
}
