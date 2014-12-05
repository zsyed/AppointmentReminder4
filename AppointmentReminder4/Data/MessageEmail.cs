using AppointmentReminder4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace AppointmentReminder.Data
{
	public class MessageEmail 
	{
		public void Send(string fromEmailAddress, string toEmailAddress, string emailSubject, string emailBody, string fromName, string ToName)
		{
            MailMessage mailMsg = new MailMessage();

            // To
            mailMsg.To.Add(new MailAddress(toEmailAddress, ToName));

            // From
            mailMsg.From = new MailAddress(fromEmailAddress, fromName);

            // Subject and multipart/alternative Body
            mailMsg.Subject = emailSubject;
            string text = emailBody;
            //string html = @"<p>html body</p>";
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            string emailHost = Security.Decrypt(ConfigurationManager.AppSettings["EmailHost"]);
            string emailPort = Security.Decrypt(ConfigurationManager.AppSettings["EmailPort"]);
            string emailAccountAddress = Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountAddress"]);
            string emailAccountPassword = Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountPassword"]);

            SmtpClient smtpClient = new SmtpClient(emailHost, Convert.ToInt32(emailPort));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(emailAccountAddress, emailAccountPassword);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);


            //System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            //mail.To.Add(toEmailAddress);
            //mail.From = new MailAddress(fromEmailAddress, fromEmailAddress, System.Text.Encoding.UTF8);
            //mail.Subject = emailSubject;
            //mail.SubjectEncoding = System.Text.Encoding.UTF8;
            //mail.Body = emailBody;
            //mail.BodyEncoding = System.Text.Encoding.UTF8;
            //mail.IsBodyHtml = true;
            //SmtpClient client = new SmtpClient();

            //string EmailAccountAddress = ConfigurationManager.AppSettings["EmailAccountAddress"];
            //string EmailAccountPassword = ConfigurationManager.AppSettings["EmailAccountPassword"];
            //string EmailHost = ConfigurationManager.AppSettings["EmailHost"];
            //// int EmailPort = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);

            //client.Credentials = new System.Net.NetworkCredential(EmailAccountAddress, EmailAccountPassword);
            //// client.Port = EmailPort;
            //client.Host = EmailHost;

            //client.EnableSsl = true;
            //client.Send(mail);
		}

        public string Decrypt { get; set; }
    }
}