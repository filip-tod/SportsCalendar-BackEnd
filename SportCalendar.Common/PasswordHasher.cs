using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Common
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            SHA256 sha256Hash = SHA256.Create();
            using (sha256Hash)
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder hashPassword = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    hashPassword.Append(bytes[i].ToString("x2"));
                }

                return hashPassword.ToString();
            }
        }
    }
}
