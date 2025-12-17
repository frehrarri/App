namespace Voyage.Utilities
{
    public class HelperMethods
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
    }
}
