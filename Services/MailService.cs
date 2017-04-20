using NinjaFit.Api.Extensions;
using NinjaFit.Api.Models;
using NinjaFit.Api.Support;
using System;
using System.Configuration;
using System.Net.Mail;

namespace NinjaFit.Api.Services
{
    public class MailService : Service
    {
        public static void Send(string subject, string content)
        {
            string to   = ConfigurationManager.AppSettings["smtp:to:email"];
            string toCC = ConfigurationManager.AppSettings["smtp:to:email:cc"];
            string from = ConfigurationManager.AppSettings["smtp:from:email"];
            string pwd  = ConfigurationManager.AppSettings["smtp:from:pwd"];

            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl             = true;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod        = SmtpDeliveryMethod.Network;
                client.Credentials           = new System.Net.NetworkCredential(from, pwd);

                client.Send(from, to, subject, content);

                if (!string.IsNullOrEmpty(toCC))
                {
                    client.Send(from, toCC, subject, content);
                }
            }
        }

        public static ApiResponse Send(Message message)
        {
            if (message == null) { return ApiResponse.Invalid("There was a problem sending message contents."); }

            #region Validation
            if (string.IsNullOrEmpty(message.FirstName))
            {
                return ApiResponse.Invalid("Please enter a first name.");
            }

            if (message.FirstName.Length < Constants.Validation.FirstNameLength)
            {
                return ApiResponse.Invalid($"First name must be at least {Constants.Validation.FirstNameLength} characters.");
            }

            if (string.IsNullOrEmpty(message.LastName))
            {
                return ApiResponse.Invalid("Please enter a last name.");
            }

            if (message.LastName.Length < Constants.Validation.LastNameLength)
            {
                return ApiResponse.Invalid($"Last name must be at least {Constants.Validation.LastNameLength} characters.");
            }

            if (string.IsNullOrEmpty(message.Email))
            {
                return ApiResponse.Invalid("Please enter an email address.");
            }

            if (!message.Email.IsValidEmail())
            {
                return ApiResponse.Invalid($"Please enter a valid email address.");
            }

            if (string.IsNullOrEmpty(message.Content))
            {
                return ApiResponse.Invalid("Please enter message content.");
            }

            if (message.Content.Length < Constants.Validation.ContentLength)
            {
                return ApiResponse.Invalid($"Message content must be at least {Constants.Validation.ContentLength} characters.");
            }
            #endregion

            string subject = $"NinjaFit Site Message from {message.FirstName} {message.LastName}";

            string content = $@"
New Message sent to NinjaFit Site

First Name: {message.FirstName}

Last Name: {message.LastName}

Email: {message.Email}

Message Content:
{message.Content}
";

            Send(subject, content);

            return ApiResponse.Valid();
        }
    }
}