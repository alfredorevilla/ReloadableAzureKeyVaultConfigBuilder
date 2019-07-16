using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Configuration.ConfigurationBuilders
{
    public class ReloadableAzureKeyVaultConfigBuilder : AzureKeyVaultConfigBuilder
    {
        private const string configBuildersKey = "configBuilders";
        private static readonly ConfigurationBuildersSection configurationBuildersSection;

        static ReloadableAzureKeyVaultConfigBuilder()
        {
            var section = ConfigurationManager.GetSection(configBuildersKey);

            if (section == null)
            {
                throw new NullReferenceException($"Section {configBuildersKey} was not found in application config.");
            }

            if (!(section is ConfigurationBuildersSection configurationBuildersSection))
            {
                throw new InvalidOperationException($"Section {configBuildersKey} is not valid.");
            }
        }

        public static void ReloadConfiguration(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            var config = configurationBuildersSection.Builders[name].Parameters;
            var builder = configurationBuildersSection.GetBuilderFromName(name);

            if (!(builder is ReloadableAzureKeyVaultConfigBuilder typeBuilder))
            {
                throw new ArgumentException($"Builder {name} type is no valid.", nameof(name));
            }

            config.Remove(preloadTag);
            config.Add(preloadTag, "true");
            typeBuilder.LazyInitialize(name, config);
        }
    }
}