using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SuperNova
{
    public  class mail
    {

        //E-mail Methods
        public static bool send_gmail(string username, string password, string to_mail, string subject, string message)
        {
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Sender E-mail should not be null");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password should not be null");
            if (string.IsNullOrEmpty(to_mail))
                throw new NullReferenceException("Reciever E-mail should not be null");
            if (string.IsNullOrEmpty(subject))
                throw new NullReferenceException("Subject should not be null");
            if (string.IsNullOrEmpty(message))
                throw new NullReferenceException("Message should not be null");

            return send_mail(username, password, to_mail, subject, message, 587, "smtp.gmail.com");
        }

        public static bool send_outlook(string username, string password, string to_mail, string subject, string message)
        {
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Sender E-mail should not be null");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password should not be null");
            if (string.IsNullOrEmpty(to_mail))
                throw new NullReferenceException("Reciever E-mail should not be null");
            if (string.IsNullOrEmpty(subject))
                throw new NullReferenceException("Subject should not be null");
            if (string.IsNullOrEmpty(message))
                throw new NullReferenceException("Message should not be null");

            return send_mail(username, password, to_mail, subject, message, 587, "smtp.live.com");
        }

        public static bool send_office365(string username, string password, string to_mail, string subject, string message)
        {
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Sender E-mail should not be null");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password should not be null");
            if (string.IsNullOrEmpty(to_mail))
                throw new NullReferenceException("Reciever E-mail should not be null");
            if (string.IsNullOrEmpty(subject))
                throw new NullReferenceException("Subject should not be null");
            if (string.IsNullOrEmpty(message))
                throw new NullReferenceException("Message should not be null");

            return send_mail(username, password, to_mail, subject, message, 587, "smtp.office365.com");
        }

        public static bool send_yahoo(string username, string password, string to_mail, string subject, string message)
        {
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Sender E-mail should not be null");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password should not be null");
            if (string.IsNullOrEmpty(to_mail))
                throw new NullReferenceException("Reciever E-mail should not be null");
            if (string.IsNullOrEmpty(subject))
                throw new NullReferenceException("Subject should not be null");
            if (string.IsNullOrEmpty(message))
                throw new NullReferenceException("Message should not be null");

            return send_mail(username, password, to_mail, subject, message, 465, "smtp.mail.yahoo.com");
        }

        public static bool send_yahoo_plus(string username, string password, string to_mail, string subject, string message)
        {
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Sender E-mail should not be null");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password should not be null");
            if (string.IsNullOrEmpty(to_mail))
                throw new NullReferenceException("Reciever E-mail should not be null");
            if (string.IsNullOrEmpty(subject))
                throw new NullReferenceException("Subject should not be null");
            if (string.IsNullOrEmpty(message))
                throw new NullReferenceException("Message should not be null");

            return send_mail(username, password, to_mail, subject, message, 465, "plus.smtp.mail.yahoo.com");
        }

        public static bool send_hotmail(string username, string password, string to_mail, string subject, string message)
        {
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Sender E-mail should not be null");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password should not be null");
            if (string.IsNullOrEmpty(to_mail))
                throw new NullReferenceException("Reciever E-mail should not be null");
            if (string.IsNullOrEmpty(subject))
                throw new NullReferenceException("Subject should not be null");
            if (string.IsNullOrEmpty(message))
                throw new NullReferenceException("Message should not be null");

            return send_mail(username, password, to_mail, subject, message, 465, "smtp.live.com");
        }

        private static bool send_mail(string username, string password, string to_mail, string subject, string message, int port, string host)
        {
            try
            {
                // Command line argument must the the SMTP host.
                SmtpClient client = new SmtpClient();
                client.Port = port;
                client.Host = host;
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(username, password);
                MailMessage mail_message = new MailMessage(username, to_mail, subject, message);
                mail_message.BodyEncoding = UTF8Encoding.UTF8;
                mail_message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mail_message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
