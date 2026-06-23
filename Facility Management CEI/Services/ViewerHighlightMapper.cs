using API.Enums;
using Facility_Management_CEI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facility_Management_CEI.Services
{
    public static class ViewerHighlightMapper
    {
        public static IReadOnlyDictionary<string, int[]> NormalizeSeverityColors(ViewerOptions options)
        {
            var configuredColors = options?.SeverityColors ?? new ViewerOptions().SeverityColors;

            return Enum.GetNames(typeof(Priority))
                .ToDictionary(
                    severity => severity,
                    severity => NormalizeColor(
                        configuredColors.TryGetValue(severity, out var color) ? color : new ViewerOptions().SeverityColors[severity]));
        }

        public static string SeverityKey(Priority priority)
        {
            return priority.ToString();
        }

        private static int[] NormalizeColor(int[] color)
        {
            var normalized = new[] { 255, 0, 0, 255 };
            if (color == null)
            {
                return normalized;
            }

            for (var i = 0; i < Math.Min(color.Length, normalized.Length); i++)
            {
                normalized[i] = Math.Max(0, Math.Min(255, color[i]));
            }

            if (color.Length == 3)
            {
                normalized[3] = 255;
            }

            return normalized;
        }
    }
}
