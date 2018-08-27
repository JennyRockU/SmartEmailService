using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendEmailApi
{
    public class EmailDetails
    {
        public List<string> Recipients;
        public string Sender;
        public string Subject;
        public string Body;

        public static EmailDetails CreateEmail(List<string> recipeints,
            string senderEmail,
            string subjectLine,
            string emailBody)
        {
            return new EmailDetails
            {
                Recipients = recipeints.Select(x => x?.Trim()).ToList(),
                Sender = senderEmail?.Trim(),
                Subject = subjectLine == null ? "" : subjectLine?.Trim(),
                Body = emailBody ?? ""
            };

        }

        public bool ValidRecipients => ValidateRecipients();
        public bool ValidSender => IsValidEmail(Sender);
        public bool ValidSubject => Subject.Length < 988;
        public bool ValidBody => Body.Length <= 2000;

        private bool ValidateRecipients()
        {
            const int maxAllowedRecipients = 50;

            return Recipients.Count() > 0 &&
                   Recipients.Count() < maxAllowedRecipients &&
                   Recipients.Any(x => IsValidEmail(x));
        }

        private bool IsValidEmail(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail)) return false;

            // check for min. length (consists of name & domain)
            var emailParts = userEmail.Split('@').Count(x => !string.IsNullOrEmpty(x));
            return emailParts > 1;
        }
        

    }
}
