using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SportCalendar.Common
{
    public class CredentialsValidation
    {
        public static bool ValidateUsername(string username)
        {
            // Regular expression pattern for allowed characters: letters (uppercase and lowercase), digits, underscore
            string pattern =  @"^[a-zA-Z0-9]+$";

            // Match the pattern against the username
            return Regex.IsMatch(username, pattern);
        }

        public static bool ValidatePassword(string password)
        {
            // Regular expression pattern for allowed characters: letters (uppercase and lowercase), digits, special characters
            string pattern = @"^[a-zA-Z0-9]+$";

            // Match the pattern against the password
            return Regex.IsMatch(password, pattern);
        }
    }
}
