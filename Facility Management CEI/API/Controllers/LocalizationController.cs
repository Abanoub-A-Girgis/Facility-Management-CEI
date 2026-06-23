using Facility_Management_CEI.ApiLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Linq;

namespace Facility_Management_CEI.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/localization")]
    public class LocalizationController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizationController(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string culture = "en")
        {
            var normalizedCulture = NormalizeCulture(culture);
            var previousCulture = CultureInfo.CurrentCulture;
            var previousUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                var cultureInfo = new CultureInfo(normalizedCulture);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;

                var translations = _localizer
                    .GetAllStrings(includeParentCultures: true)
                    .GroupBy(item => item.Name)
                    .ToDictionary(group => group.Key, group => group.First().Value);

                var response = new LocalizationDto
                {
                    Culture = normalizedCulture,
                    Direction = normalizedCulture == "ar" ? "rtl" : "ltr",
                    Translations = translations
                };

                return Ok(ApiResponse<LocalizationDto>.Ok(response));
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUiCulture;
            }
        }

        private static string NormalizeCulture(string culture)
        {
            return culture != null && culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
                ? "ar"
                : "en";
        }
    }
}
