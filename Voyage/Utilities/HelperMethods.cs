using Ganss.Xss;

namespace Voyage.Utilities
{
    public static class HelperMethods
    {

        public static string AddSpacesToSentence(string sentence, int spaces = 1)
        {
            if (string.IsNullOrWhiteSpace(sentence))
                return sentence;

            // Insert space before each uppercase letter that follows a lowercase letter
            var spaced = System.Text.RegularExpressions.Regex.Replace(sentence, "([a-z])([A-Z])", "$1 $2");

            // Optional: replace underscores with spaces too
            spaced = spaced.Replace("_", " ");

            return spaced;
        }

        public static string SanitizeHtmlForXSS(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            HtmlSanitizer sanitizer = new HtmlSanitizer();

            // Start from zero trust
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedCssProperties.Clear();
            sanitizer.AllowedSchemes.Clear();

            // Allowed formatting tags for ticket notes
            sanitizer.AllowedTags.Add("br");
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("u");

            return sanitizer.Sanitize(html);
        }
        public static string FormatUtcDateTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            // Ensure it's UTC
            DateTime? utcDate = dateTime.Value.Kind == DateTimeKind.Utc ? dateTime: dateTime.Value.ToUniversalTime();

            return utcDate.Value.ToString("dd MMM yyyy HH:mm");
        }


        public static string FormatUtcDate(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            // Ensure it's UTC
            DateTime? utcDate = dateTime.Value.Kind == DateTimeKind.Utc ? dateTime : dateTime.Value.ToUniversalTime();

            return utcDate.Value.ToString("dd MMM yyyy");
        }















    }
}
