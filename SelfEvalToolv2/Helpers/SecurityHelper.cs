using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SelfEvalToolv2.Helpers
{
    public class SecurityHelper
    {
        public static void SendPasswordForgottenEmail(string token, string recipientEmail)
        {
            var smtp = new SmtpClient();
            string mailfrom = (ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection).From;
            string mailto = recipientEmail;
            string result = "An email has been send to '" + mailto + "' with details to reset your password.";
            string baseUrl = HttpContext.Current.Request.Url.Scheme + System.Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host + (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port);
            string body = string.Format(@"
Thank you for using the FRINDOC tool. We received a password reset request for this email address ({0}) on {2}. To reset your password, click the link below:

{2}/Account/ResetPassword/{1}

If you did not request a password reset, then someone probably mis-typed their email address. You can ignore this message, and we apologize for the inconvenience.

If you have any problems resetting your password, you can get help from us at frindoc@eua.be.


Kind regards,

The FRINDOC Team  


", mailto, token, baseUrl);
            var mail = new MailMessage(mailfrom, mailto, "FRINDOC - Password Reset", body);
            smtp.Send(mail);
        }

        public static void SendCreatedSubaccountEmail(string token, string recipientEmail, string username)
        {
            var smtp = new SmtpClient();
            string mailfrom = (ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection).From;
            string mailto = recipientEmail;
            string result = "An email has been send to '" + mailto + "' with details to reset your password.";
            string baseUrl = HttpContext.Current.Request.Url.Scheme + System.Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host + (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port);
            string body = string.Format(@"
Dear {2},

Thank you for using the FRINDOC tool. Someone has created a subaccount for you ({0}) on {3}. To set your password, please click the link below:

{3}/Account/ResetPassword/{1}

If you feel this wasn't meant for you, someone may have entered your email address by mistake. You can ignore this message, and we apologize for the inconvenience.

If you have any problems resetting your password, you can contact us by email at frindoc@eua.be.


Kind regards,

The FRINDOC Team  

", mailto, token, username, baseUrl);
            var mail = new MailMessage(mailfrom, mailto, "FRINDOC - Account created", body);
            smtp.Send(mail);
        }
    }
}