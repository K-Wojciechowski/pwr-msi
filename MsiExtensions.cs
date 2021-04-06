using System;
using Microsoft.Extensions.Configuration;

namespace pwr_msi {
    public static class MsiExtensions {
        public static string GetString(this IConfiguration configuration, string key, string defaultValue) {
            return configuration.GetValue(key, defaultValue);
        }

        public static int GetInt(this IConfiguration configuration, string key, int defaultValue) {
            return configuration.GetValue(key, defaultValue);
        }

        public static bool GetBoolean(this IConfiguration configuration, string key, bool defaultValue) {
            var stringValue = configuration.GetString(key, defaultValue: null);
            if (stringValue == null) return defaultValue;
            var charValue = stringValue.ToLower()[index: 0];
            return charValue == '1' || charValue == 'y' || charValue == 't';
        }

        public static TimeSpan GetTimeSpan(this IConfiguration configuration, string key, TimeSpan defaultValue) {
            var sentinel = -1;
            var intValue = configuration.GetInt(key, sentinel);
            return intValue == sentinel ? defaultValue : TimeSpan.FromSeconds(intValue);
        }
    }
}
