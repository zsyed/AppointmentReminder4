using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentReminder4.Data
{
    public static class Security
    {

        public static string Encrypt(string text)
        {
            string encrypted = string.Empty;
            foreach (char c in text)
            {
                char s = (char)(c - 1);
                encrypted += s.ToString();
            }
            return encrypted;
        }

        public static string Decrypt(string text)
        {
            string decrypted = string.Empty;
            foreach (char c in text)
            {
                char s = (char)(c + 1);
                decrypted += s.ToString();
            }
            return decrypted;
        }

    }
}