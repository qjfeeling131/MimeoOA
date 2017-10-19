using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Abp.Utilities
{
    public class RegUtility
    {

        public static bool IsValidationForEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static bool IsValidationForPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^[1]+[3,5]+\d{9}", RegexOptions.IgnoreCase);
        }

        public static bool IsValidationForCardNumber(string cardNumber)
        {
            return Regex.IsMatch(cardNumber, @"(^\d{18}$)|(^\d{15}$)");
        }
    }
}
