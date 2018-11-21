using System;
using Xunit;

namespace Altairis.Services.Mailing.Templating.Tests {
    public class DefaultReplacementServiceTests {

        [Fact]
        public void SimpleReplacement() {
            var rs = new TemplateReplacer(new { just = "JUST", test = "TEST" });
            var r = rs.ReplacePlaceholders("This is {{just}} a {{test}}.");
            Assert.Equal("This is JUST a TEST.", r);
        }

        [Fact]
        public void FormattedReplacement() {
            var rs = new TemplateReplacer(new { date = DateTime.MinValue });
            var r = rs.ReplacePlaceholders("{{date:R}}");
            Assert.Equal("Mon, 01 Jan 0001 00:00:00 GMT", r);
        }

        [Fact]
        public void NonExistingKey() {
            var ex = Assert.Throws<FormatException>(() => {
                var rs = new TemplateReplacer(new { just = "JUST", test = "TEST" });
                var r = rs.ReplacePlaceholders("Requesting {{nonexisting}} key.");
            });
            Assert.Equal("Requested key 'nonexisting' was not found in supplied values.", ex.Message);
        }

        [Fact]
        public void NotFormattableValueKey() {
            var ex = Assert.Throws<FormatException>(() => {
                var rs = new TemplateReplacer(new { key = new object() });
                var r = rs.ReplacePlaceholders("Testing {{key:N0}} with no formatting.");
            });
            Assert.Equal("Value for key 'key' is not IFormattable, but custom format string was provided.", ex.Message);
        }

    }
}
