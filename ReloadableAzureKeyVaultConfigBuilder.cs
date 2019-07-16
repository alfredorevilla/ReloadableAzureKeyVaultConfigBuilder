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

        public static void ReloadConfiguration(string builderName)
        {
            if (string.IsNullOrWhiteSpace(builderName))
            {
                throw new ArgumentException(nameof(builderName));
            }

            var config = configurationBuildersSection.Builders[builderName].Parameters;
            var builder = configurationBuildersSection.GetBuilderFromName(builderName);

            if (!(builder is ReloadableAzureKeyVaultConfigBuilder typedBuilder))
            {
                throw new ArgumentException($"Builder {builderName} is not valid.", nameof(builderName));
            }

            config.Remove(preloadTag);
            config.Add(preloadTag, "true");
            typedBuilder.LazyInitialize(builderName, config);
        }
    }
}