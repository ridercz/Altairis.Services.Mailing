using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Altairis.Services.Mailing.Templating {
    public class TemplateReplacer {
        private readonly CultureInfo culture;
        private const string PlaceholderPattern = @"\{\{.*?\}\}";
        private readonly Dictionary<string, IFormattable> formattableValues = new Dictionary<string, IFormattable>();
        private readonly Dictionary<string, string> unformattableValues = new Dictionary<string, string>();

        public TemplateReplacer(object values, CultureInfo culture = null) {
            if (values == null) throw new ArgumentNullException(nameof(values));

            this.culture = culture ?? CultureInfo.CurrentCulture;


            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values)) {
                var rawValue = descriptor.GetValue(values);

                if (rawValue is IFormattable) {
                    this.formattableValues.Add(descriptor.Name.ToLower(), rawValue as IFormattable);
                }
                else {
                    this.unformattableValues.Add(descriptor.Name.ToLower(), rawValue?.ToString());
                }
            }
        }

        public string ReplacePlaceholders(string template) {
            if (string.IsNullOrWhiteSpace(template)) return template;

            var result = Regex.Replace(template, PlaceholderPattern, m => this.GetReplacementValue(m.Value));
            return result;
        }

        private string GetReplacementValue(string placeholder) {
            if (placeholder == null) throw new ArgumentNullException(nameof(placeholder));
            if (placeholder.Length < 4) throw new ArgumentException("Placeholder value is too short.", nameof(placeholder));
            if (string.IsNullOrWhiteSpace(placeholder)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(placeholder));
            if (placeholder.Length == 4) return string.Empty;

            // Remove {{...}}
            placeholder = placeholder.Trim('{', '}');

            // Get format string and property name
            string key, formatString = null;
            if (placeholder.Contains(":")) {
                var data = placeholder.Split(new char[] { ':' }, 2);
                key = data[0];
                formatString = data[1];
            }
            else {
                key = placeholder.ToLower();
            }

            if (string.IsNullOrWhiteSpace(formatString)) {
                // Unformatted value
                if (this.unformattableValues.ContainsKey(key)) return this.unformattableValues[key];
                if (this.formattableValues.ContainsKey(key)) return this.formattableValues[key]?.ToString(null, this.culture);
            }
            else {
                // Formatted value
                if (this.formattableValues.ContainsKey(key)) return this.formattableValues[key]?.ToString(formatString, this.culture);
                if (this.unformattableValues.ContainsKey(key)) throw new FormatException($"Value for key '{key}' is not IFormattable, but custom format string was provided.");
            }
            throw new FormatException($"Requested key '{key}' was not found in supplied values.");
        }

    }
}
