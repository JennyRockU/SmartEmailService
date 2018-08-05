using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendEmailApi
{
    public class EmailDetails
    {
        public IEnumerable<string> Recipients;
        public string Sender;
        public string Subject;
        public string Body;

        public EmailDetails(IEnumerable<string> recipeints, string senderEmail, string subjectLine,
            string emailBody)
        {
            this.Recipients = recipeints.Select(x => x?.Trim());
            this.Sender = senderEmail?.Trim();
            this.Subject = subjectLine == null ? "" : subjectLine?.Trim();
            this.Body = emailBody ?? "";
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

        public string GetMessageQueueString()
        {
            return $"<To>{string.Join(",", Recipients)}</To>" +
                $"<From>{Sender}</From>" +
                $"<Subject>{Subject}</Subject>" +
                $"<Body>{Body}</Body>";
        }

        public static EmailDetails UnwrapMessage(string msgFromQueue)
        {
            var recipients = GetTextWithTag("To", msgFromQueue);
            var sender = GetTextWithTag("From", msgFromQueue);
            var subject = GetTextWithTag("Subject", msgFromQueue);
            var body = GetTextWithTag("Body", msgFromQueue);

            return new EmailDetails(recipients.Split(','), sender, subject, body);

        }

        private static string GetTextWithTag(string tagName, string text)
        {
            var openingTagIndex = text.IndexOf($"<{tagName}>");
            var lastIndexOfOpeningTag = text.Select((c, x) => new { Char = c, Index = x }).ToList().FindIndex(x => x.Index > openingTagIndex && x.Char.Equals('>'));
            var startIndex = lastIndexOfOpeningTag + 1;
            var closingTagIndex = text.IndexOf($"</{tagName}>");
            var endIndex = closingTagIndex - startIndex;

            return text.Substring(startIndex, endIndex);
        }

    }

   
}