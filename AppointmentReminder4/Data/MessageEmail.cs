using AppointmentReminder4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace AppointmentReminder.Data
{
	public class MessageEmail 
	{

        public Task SendAsync(string fromEmailAddress, string toEmailAddress, string emailSubject, string emailBody, string fromName, string ToName)
        {
            Send(fromEmailAddress, toEmailAddress, emailSubject, emailBody, fromName, ToName);
            return Task.FromResult(0);
        }

		public void Send(string fromEmailAddress, string toEmailAddress, string emailSubject, string emailBody, string fromName, string ToName)
		{

            MailMessage mailMsg = new MailMessage();

            // To
            mailMsg.To.Add(new MailAddress(toEmailAddress, ToName));

            // From
            // mailMsg.From = new MailAddress(fromEmailAddress, fromName);

            // Subject and multipart/alternative Body
            mailMsg.Subject = emailSubject;
            string text = emailBody;
            // string html = @"<p>html body</p>";
            // mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            string emailHost = Security.Decrypt(ConfigurationManager.AppSettings["EmailHost"]);
            string emailPort = Security.Decrypt(ConfigurationManager.AppSettings["EmailPort"]);
            string emailAccountAddress = Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountAddress"]);
            string emailAccountPassword = Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountPassword"]);

            mailMsg.From = new MailAddress(emailAccountAddress, fromName);

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient(emailHost, Convert.ToInt32(emailPort));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(emailAccountAddress, emailAccountPassword);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);



            ////MailMessage mailMsg = new MailMessage();

            ////// To
            ////mailMsg.To.Add(new MailAddress(toEmailAddress, ToName));

            ////// From
            ////mailMsg.From = new MailAddress(fromEmailAddress, fromName);

            ////// Subject and multipart/alternative Body
            ////mailMsg.Subject = emailSubject;
            ////string text = emailBody;
            ////mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Html));

            ////SmtpClient smtpClient = new SmtpClient(emailHost, Convert.ToInt32(emailPort));
            ////System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(emailAccountAddress, emailAccountPassword);
            ////smtpClient.Credentials = credentials;
            ////smtpClient.EnableSsl = true;
            ////smtpClient.UseDefaultCredentials = false;
            ////smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            ////smtpClient.Send(mailMsg);
		}

        public string Decrypt { get; set; }
    }
}