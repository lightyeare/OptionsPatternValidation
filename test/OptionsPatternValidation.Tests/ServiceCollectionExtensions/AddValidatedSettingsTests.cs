using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.TestHelpers;
using OptionsPatternValidation.Tests.TestSettings;
using OptionsPatternValidation.Tests.TestSettings.AttributeValidation;
using OptionsPatternValidation.Tests.TestSettingsJson;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Test the DataAnnotations AddValidatedSettings(config) method.</summary>
    public class AddValidatedSettingsTests
    {
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings_from_string()
        {
            var services = new ServiceCollection();

            
            const string json = @"
{
""AttributeValidated"": {
    ""IntegerA"": 76,
    ""BooleanB"": true
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(76, result.IntegerA);
        }
        
        [Fact]
        public void Catches_validation_error_for_Test1_file()
        {
            var services = new ServiceCollection();
            
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(
                JsonIndex.AttributeValidated.Test1);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);
            
            var serviceProvider = services.BuildServiceProvider();

            // this call will work, because we're just getting the accessor singleton
            var optionsAccessor = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>();
            Action act = () =>
            {
                var result = optionsAccessor.Value;
            };
            
            var exception = Assert.Throws<OptionsValidationException>(act);
            Assert.StartsWith("Validation failed for members: 'IntegerA'", exception.Message);
        }

        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings_from_Test2_file()
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(
                JsonIndex.AttributeValidated.Test2);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(92, result.IntegerA);
        }
    }
}