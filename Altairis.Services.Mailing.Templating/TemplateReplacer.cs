using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Altairis.Services.Mailing.Templating;

public partial class TemplateReplacer {
    private readonly CultureInfo culture;
    private const string PlaceholderPattern = @"\{\{.*?\}\}";
    private readonly Dictionary<string, IFormattable> formattableValues = [];
    private readonly Dictionary<string, string> unformattableValues = [];

    public TemplateReplacer(object values, CultureInfo? culture = null) {
        ArgumentNullException.ThrowIfNull(values);

        this.culture = culture ?? CultureInfo.CurrentCulture;

        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values)) {
            var rawValue = descriptor.GetValue(values);

            if (rawValue is IFormattable) {
                this.formattableValues.Add(descriptor.Name.ToLower(), rawValue as IFormattable ?? throw new InvalidOperationException($"Value for key '{descriptor.Name}' is not IFormattable."));
            } else {
                this.unformattableValues.Add(descriptor.Name.ToLower(), rawValue?.ToString() ?? string.Empty);
            }
        }
    }

    public string ReplacePlaceholders(string template) {
        if (string.IsNullOrWhiteSpace(template)) return template;

        var result = PlaceholderRegex().Replace(template, m => this.GetReplacementValue(m.Value) ?? string.Empty);
        return result;
    }

    private string? GetReplacementValue(string placeholder) {
        ArgumentNullException.ThrowIfNull(placeholder);
        if (placeholder.Length < 4) throw new ArgumentException("Placeholder uValue is too short.", nameof(placeholder));
        if (string.IsNullOrWhiteSpace(placeholder)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(placeholder));
        if (placeholder.Length == 4) return string.Empty;

        // Remove {{...}}
        placeholder = placeholder.Trim('{', '}');

        // Get format string and property name
        string key;
        string? formatString = null;
        if (placeholder.Contains(':')) {
            var data = placeholder.Split([':'], 2);
            key = data[0];
            formatString = data[1];
        } else {
            key = placeholder.ToLower();
        }

        if (string.IsNullOrWhiteSpace(formatString)) {
            // Unformatted value
            if (this.unformattableValues.TryGetValue(key, out var uValue)) return uValue;
            if (this.formattableValues.TryGetValue(key, out var fValue)) return fValue?.ToString(null, this.culture);
        } else {
            // Formatted value
            if (this.formattableValues.TryGetValue(key, out var fValue)) return fValue?.ToString(formatString, this.culture);
            if (this.unformattableValues.ContainsKey(key)) throw new FormatException($"Value for key '{key}' is not IFormattable, but custom format string was provided.");
        }
        throw new FormatException($"Requested key '{key}' was not found in supplied values.");
    }

    [GeneratedRegex(PlaceholderPattern)]
    private static partial Regex PlaceholderRegex();
}
