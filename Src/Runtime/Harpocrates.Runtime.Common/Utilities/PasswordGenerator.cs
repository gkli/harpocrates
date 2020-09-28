using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Utilities
{
    public class PasswordGenerator
    {
        [Flags]
        public enum PasswordProperties
        {
            LowerCase = 1,
            UpperCase = 2,
            Numeric = 4,
            SpecialCharacters = 8,
            Spaces = 16,
            Strong = LowerCase | UpperCase | Numeric | SpecialCharacters
        }

        private static class CharacterSet
        {
            public const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
            public const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public const string Numeric = "0123456789";
            public const string Special = @"!#$%&*@\";
            public const string Space = " ";
        }

        private static class Defaults
        {
            public const int MaxIdenticalConsecutiveChars = 2;
            public const int MinPasswordLength = 8;
            public const int MaxPasswordLength = 256;
        }

        public static string GeneratePassword(int lengthOfPassword = 16, PasswordProperties properties = PasswordProperties.Strong)
        {
            if (lengthOfPassword < Defaults.MinPasswordLength || lengthOfPassword > Defaults.MaxIdenticalConsecutiveChars)
            {
                throw new InvalidOperationException($"Password length must be between { Defaults.MinPasswordLength} and {Defaults.MaxIdenticalConsecutiveChars}.");
            }

            string characterSet = GetPasswordCharacterSet(properties);

            char[] password = new char[lengthOfPassword];
            int characterSetLength = characterSet.Length;

            Random random = new Random();
            for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
            {
                password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];

                bool moreThanTwoIdenticalInARow =
                    characterPosition > Defaults.MaxIdenticalConsecutiveChars
                    && password[characterPosition] == password[characterPosition - 1]
                    && password[characterPosition - 1] == password[characterPosition - 2];

                if (moreThanTwoIdenticalInARow)
                {
                    characterPosition--;
                }
            }

            return string.Join(null, password);
        }

        private static string GetPasswordCharacterSet(PasswordProperties properties)
        {

            StringBuilder characterSet = new StringBuilder();

            if ((properties & PasswordProperties.LowerCase) > 0)
            {
                characterSet.Append(CharacterSet.LowerCase);
            }

            if ((properties & PasswordProperties.UpperCase) > 0)
            {
                characterSet.Append(CharacterSet.UpperCase);
            }

            if ((properties & PasswordProperties.Numeric) > 0)
            {
                characterSet.Append(CharacterSet.Numeric);
            }

            if ((properties & PasswordProperties.SpecialCharacters) > 0)
            {
                characterSet.Append(CharacterSet.Special);
            }

            if ((properties & PasswordProperties.Spaces) > 0)
            {
                characterSet.Append(CharacterSet.Space);
            }

            return characterSet.ToString();
        }
    }
}
