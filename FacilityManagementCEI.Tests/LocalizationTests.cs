using Facility_Management_CEI;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;
using Xunit;

namespace FacilityManagementCEI.Tests
{
    public class LocalizationTests
    {
        [Theory]
        [InlineData("en")]
        [InlineData("ar")]
        public void SharedResourcesCanBeEnumerated(string culture)
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions { ResourcesPath = "Resources" }),
                NullLoggerFactory.Instance);
            var localizer = factory.Create(typeof(SharedResource));
            var previousCulture = CultureInfo.CurrentCulture;
            var previousUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = new CultureInfo(culture);

                var translations = localizer
                    .GetAllStrings(includeParentCultures: true)
                    .ToDictionary(item => item.Name, item => item.Value);

                Assert.True(translations.TryGetValue("App.Title", out var title));
                Assert.False(string.IsNullOrWhiteSpace(title));
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUiCulture;
            }
        }
    }
}
