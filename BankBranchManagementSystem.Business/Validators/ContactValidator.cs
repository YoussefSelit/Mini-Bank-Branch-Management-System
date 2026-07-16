using System.Net.Mail;
using PhoneNumbers;

using System.Text.RegularExpressions;

namespace BankBranchManagementSystem.Validators
{
    public static class ContactValidator
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /*public static bool IsValidPhone(string phone)
        {
            var util = PhoneNumberUtil.GetInstance();

            try
            {
                var number = util.Parse(phone, "EG"); // EG = Egypt
                return util.IsValidNumber(number);
            }
            catch
            {
                return false;
            }
        }*/

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var cleaned = Regex.Replace(phone, @"[\s\-\(\)]", "");

            return Regex.IsMatch(cleaned, @"^\+?\d{7,15}$");
        }
    }   
}