using Ganss.Xss;

namespace Helper.Infrastructure
{
    public static class StringHelper
    {
        //string min/max and security validator
        public static bool Validator(this string value, int minLenght, int maxLenght)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return false;

            HtmlSanitizer sanitizer = new HtmlSanitizer();
            value = sanitizer.Sanitize(value);

            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Length >= minLenght || value.Length <= maxLenght)
                return false;

            return true;
        }
    }
}