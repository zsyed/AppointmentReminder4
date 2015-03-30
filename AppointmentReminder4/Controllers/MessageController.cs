using AppointmentReminder.Data;
using AppointmentReminder4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Twilio;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net;
using System.IO;
using System.Web.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using AppointmentReminder4.Models;
using AppointmentReminder4.Providers;
using AppointmentReminder4.Results;
using StructureMap;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using AppointmentReminder4.Common;


namespace AppointmentReminder4.Controllers
{ 
    public class MessageController : Controller
    {
        //
        // GET: /Message/ 

        private IReminderDb _db;

        public enum TimeZone
        {
            PST, 
            MST,
            CST,
            EST
        }

        public MessageController(IReminderDb db)
        {
            _db = db;
        }

        public string CurrentDateTimeValue(string timeZone)
        {
            int prodServerTimeDifference = 0;


            var enumTimeZones = Enum.GetNames(typeof(TimeZone));

            foreach (var enumTimeZone in enumTimeZones)
            {
                if (enumTimeZone.Equals(timeZone))
                {
                    prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings[enumTimeZone]);
                    break;
                }
            }

            if (prodServerTimeDifference != 0)
            {
                return DateTime.Now.AddHours(prodServerTimeDifference).ToString();
            }
            else
            {
                return null;
            }
        }

        public string CurrentDateTime()
        {
            return DateTime.Now.ToString();
        }

        public JsonResult SendReminderTest()
        {
            // For each reminder
            var contactList = new List<SelectListItem>();
            try
            {
                var reminders = new ReminderDb().Reminders;

                foreach (var reminder in reminders)
                {
                    if (reminder.ContactId == 0 && !reminder.Sent)
                    {
                        var contact = new ReminderDb().Contacts.Where(c => c.ProfileId == reminder.ProfileId).FirstOrDefault();
                        DateTime serverCurrentDateTime = this.ServerCurrentDateTime(reminder);
                        if (SendOnceTest(contact, reminder, serverCurrentDateTime))
                        {
                            contactList.Add(new SelectListItem()
                            {
                                Text = string.Format("{0} {1}", "Home", "Test"),
                                Value = reminder.Message
                            });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                contactList.Add(new SelectListItem() { Selected = false, Text = exception.Message, Value = exception.InnerException.ToString() });
            }

            if (contactList.Count == 0)
            {
                contactList.Add(new SelectListItem() { Selected = false, Text = "No appointments to send out", Value = "none" });
            }

            return Json(contactList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SendReminder()
        {
            // For each reminder loop through.
            var contactList = new List<SelectListItem>();
            try
            {
                var reminders = new ReminderDb().Reminders;

                foreach (var reminder in reminders)
                {
                    var contact = new ReminderDb().Contacts.Where(c => c.Id == reminder.ContactId).FirstOrDefault();
                    if (contact != null)
                    {
                        if (this.SendReminderAllowed(reminder))
                        {
                            // get server current datetime.
                            DateTime serverCurrentDateTime = this.ServerCurrentDateTime(reminder);

                            if (SendDaily(contact, reminder, serverCurrentDateTime))
                            {
                                contactList.Add(new SelectListItem()
                                {
                                    Text = string.Format("{0} {1}", contact.FirstName.Trim(), contact.LastName.Trim()),
                                    Value = reminder.Message
                                });
                            }

                            if (SendWeekly(contact, reminder, serverCurrentDateTime))
                            {
                                contactList.Add(new SelectListItem()
                                {
                                    Text = string.Format("{0} {1}", contact.FirstName.Trim(), contact.LastName.Trim()),
                                    Value = reminder.Message
                                });
                            }

                            if (SendOnce(contact, reminder, serverCurrentDateTime))
                            {
                                contactList.Add(new SelectListItem()
                                {
                                    Text = string.Format("{0} {1}", contact.FirstName.Trim(), contact.LastName.Trim()),
                                    Value = reminder.Message
                                });
                            }
                        }
                    }
                    //else if (reminder.ProfileId == 0 & reminder.ContactId == 0)
                    //{
                    //    contact = new ReminderDb().Contacts.Where(c => c.ProfileId == 0).FirstOrDefault();
                    //    DateTime serverCurrentDateTime = this.ServerCurrentDateTime(reminder);
                    //    if (SendOnceTest(contact, reminder, serverCurrentDateTime))
                    //    {
                    //        contactList.Add(new SelectListItem()
                    //        {
                    //            Text = string.Format("{0} {1}", "Home", "Test"),
                    //            Value = reminder.Message
                    //        });
                    //    }
                    //}
                }
            }
            catch (Exception exception)
            {
                contactList.Add(new SelectListItem() { Selected = false, Text = exception.Message, Value = exception.InnerException.ToString() });
            }

            if (contactList.Count == 0)
            {
                contactList.Add(new SelectListItem() { Selected = false, Text = "No appointments to send out", Value = "none" });
            }

            return Json(contactList, JsonRequestBehavior.AllowGet);

        }

        private bool SendOnce(Contact contact, Reminder reminder, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;
            if (reminder.Recurrence == Frequency.Once)
            {
                var profile = _db.Profiles.ToList().Find(p => p.Id == reminder.ProfileId);

                TimeSpan timeDifference = reminder.ReminderDateTime.TimeOfDay - serverCurrentDateTime.TimeOfDay;
                int RemdinerMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RemdinerMinutes"]);

                if ((reminder.ReminderDateTime.Date == serverCurrentDateTime.Date) && (reminder.ReminderDateTime.Hour == serverCurrentDateTime.Hour) && (timeDifference.Minutes >= 0 && timeDifference.Minutes <= RemdinerMinutes))
                {
                    reminderSent = SendNotification(contact, reminder, profile, serverCurrentDateTime);
                }
            }
            return reminderSent;
        }

        private bool SendOnceTest(Contact contact, Reminder reminder, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;
            if (reminder.Recurrence == Frequency.Once && contact.ProfileId == reminder.ProfileId && reminder.ContactId == 0 && reminder.Sent == false)
            {
                TimeSpan timeDifference = reminder.ReminderDateTime.TimeOfDay - serverCurrentDateTime.TimeOfDay;
                int RemdinerMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RemdinerMinutes"]);

                if ((reminder.ReminderDateTime.Date == serverCurrentDateTime.Date) && (reminder.ReminderDateTime.Hour == serverCurrentDateTime.Hour) && (timeDifference.Minutes >= 0 && timeDifference.Minutes <= RemdinerMinutes))
                {
                    reminderSent = SendNotificationTest(contact, reminder, serverCurrentDateTime);
                }
            }
            return reminderSent;
        }

        private bool SendDaily(Contact contact, Reminder reminder, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;
            if (reminder.Recurrence == Frequency.Daily)
            {
                var profile = _db.Profiles.ToList().Find(p => p.Id == reminder.ProfileId);

                TimeSpan timeDifference = reminder.ReminderDateTime.TimeOfDay - serverCurrentDateTime.TimeOfDay;
                int RemdinerMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RemdinerMinutes"]);

                if ((reminder.ReminderDateTime.Hour == serverCurrentDateTime.Hour) && (timeDifference.Minutes >= 0 && timeDifference.Minutes <= RemdinerMinutes))
                {
                    reminderSent = SendNotification(contact, reminder, profile, serverCurrentDateTime);
                }
            }
            return reminderSent;
        }

        private bool SendWeekly(Contact contact, Reminder reminder, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;
            if (reminder.Recurrence == Frequency.Weekly)
            {
                var profile = _db.Profiles.ToList().Find(p => p.Id == reminder.ProfileId);

                TimeSpan timeDifference = reminder.ReminderDateTime.TimeOfDay - serverCurrentDateTime.TimeOfDay;
                int RemdinerMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["RemdinerMinutes"]);

                if ((reminder.WeekDay == serverCurrentDateTime.DayOfWeek.ToString()) && (reminder.ReminderDateTime.Hour == serverCurrentDateTime.Hour) && (timeDifference.Minutes >= 0 && timeDifference.Minutes <= RemdinerMinutes))
                {
                    reminderSent = SendNotification(contact, reminder, profile, serverCurrentDateTime);
                }
            }
            return reminderSent;
        }

        private bool SendNotification(Contact contact, Reminder reminder, Profile profile, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;

            if (contact.SendEmail)
            {
                this.SendEmailMessage(reminder, profile, contact);
                RecordSendEmailHistory(reminder, contact, profile, serverCurrentDateTime);
                reminderSent = true;
            }

            if (contact.SendSMS)
            {
                this.SendSMSMessage(reminder, profile, contact);
                RecordSendSMSHistory(reminder, contact, profile, serverCurrentDateTime);
                reminderSent = true;
            }

            if (reminderSent)
            {
                RecordReminderSent(reminder);
            }

            return reminderSent;
        }

        private bool SendNotificationTest(Contact contact, Reminder reminder, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;

            if (contact.SendEmail)
            {
                this.SendEmailMessageTest(reminder, contact);
                RecordSendEmailHistoryTest(reminder, contact, serverCurrentDateTime);
                reminderSent = true;
            }

            if (contact.SendSMS)
            {
                this.SendSMSMessageTest(reminder, contact);
                RecordSendSMSHistoryTest(reminder, contact, serverCurrentDateTime);
                reminderSent = true;
            }

            if (reminderSent)
            {
                RecordReminderSent(reminder);
            }

            return reminderSent;
        }

        private string EmailMessageToSend(Reminder reminder, Profile profile, Contact contact)
        {
            return MessageToSend(reminder, profile, contact);
        }

        private string EmailMessageToSendTest(Reminder reminder, Contact contact)
        {
            return MessageToSendTest(reminder,contact);
        }

        private string SMSMessageToSend(Reminder reminder, Profile profile, Contact contact)
        {
            return MessageToSendSMS(reminder, profile, contact);
        }

        private string SMSMessageToSendTest(Reminder reminder, Contact contact)
        {
            return MessageToSendSMSTest(reminder, contact);
        }

        private string MessageToSendTest(Reminder reminder, Contact contact)
        {
            return string.Format("Hi {0},{1}{2}{1}Sincerely,{1}{3}{1}{1}This email was sent by {4}", contact.FirstName.Trim(), "</br>", reminder.Message, "Site Admin.", ConfigurationManager.AppSettings["WebSiteName"]);
        }

        private string MessageToSend(Reminder reminder, Profile profile, Contact contact)
        {
            return string.Format("Hi {0},{1}{2}{1}Sincerely,{1}{3}{1}{1}This email was sent by {4}", contact.FirstName.Trim(), "</br>", reminder.Message, profile.FirstName, ConfigurationManager.AppSettings["WebSiteName"]);
        }

        private string MessageToSendSMS(Reminder reminder, Profile profile, Contact contact)
        {
            return string.Format("Hi {0},{1}{2}{1}Sincerely,{1}{3}{1}{1}{1}{4}{1}{1}Message Sent to you by {5}", contact.FirstName.Trim(), "\n", reminder.Message, profile.FirstName, "text back STOP to end these reminders.", ConfigurationManager.AppSettings["WebSiteName"]);
        }

        private string MessageToSendSMSTest(Reminder reminder, Contact contact)
        {
            return string.Format("Hi {0},{1}{2}{1}Sincerely,{1}{3}{1}{1}{1}{4}{1}{1}Message Sent to you by {5}", contact.FirstName.Trim(), "\n", reminder.Message, "Test", "text back STOP to end these reminders.", ConfigurationManager.AppSettings["WebSiteName"]);
        }


        private void SendEmailMessage(Reminder reminder, Profile profile, Contact contact)
        {
            string fromEmailAddress = profile.EmailAddress;
            string toEmailAddress = contact.EmailAddress;
            string emailSubject = reminder.EmailSubject; // string.Format("Reminder from {0} {1} - {2}", profile.FirstName, profile.LastName, DateTime.Now.ToString());

            var callbackUrl = string.Format("/{0}/{1}?EmailAddress={2}","Message", "AddEmailAddressToUnsubscribeList", toEmailAddress); // "Message/AddEmailAddressToUnsubscribeList?EmailAddress=datagig@gmail.com
            callbackUrl = "http://" + Request.Url.Authority + callbackUrl;
            var callBackMessage = "Unsubscribe by clicking <a href=\"" + callbackUrl + "\">here</a>";

            string emailBody = this.EmailMessageToSend(reminder, profile, contact);
            emailBody = string.Format("<p>{0}</p>{1}{1}<p>{2}</p>", emailBody, "</br>", callBackMessage);
            var emailMessage = new MessageEmail();
            emailMessage.Send(fromEmailAddress, toEmailAddress, emailSubject, emailBody, profile.FirstName + " " + profile.LastName, contact.FirstName + " " + contact.LastName);
        }

        private void SendEmailMessageTest(Reminder reminder, Contact contact)
        {
            string fromEmailAddress = "TestEmail";
            string toEmailAddress = contact.EmailAddress;
            string emailSubject = reminder.EmailSubject; // string.Format("Reminder from {0} {1} - {2}", profile.FirstName, profile.LastName, DateTime.Now.ToString());

            string emailBody = this.EmailMessageToSendTest(reminder, contact);
            emailBody = string.Format("<p>{0}</p>{1}{1}", emailBody, "</br>");
            var emailMessage = new MessageEmail();
            emailMessage.Send(fromEmailAddress, toEmailAddress, emailSubject, emailBody, "Test Email", "Test Email");
        }

        private void SendSMSMessage(Reminder reminder, Profile profile, Contact contact)
        {
            string fromPhoneNumber = string.Format("+1{0}", profile.PhoneNumberIssued);
            string toPhoneNumber = contact.PhoneNumber;
            string message = this.SMSMessageToSend(reminder, profile, contact);
            string image = reminder.Image;

            string AccountSid = profile.AccountSid;
            string AuthToken = Security.Decrypt(profile.AuthToken);

            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            if (string.IsNullOrEmpty(image))
            {
                twilio.SendMessage(fromPhoneNumber, toPhoneNumber, message);
            }
            else
            {
                twilio.SendMessage(fromPhoneNumber, toPhoneNumber, message, new string[] { image });
            }

            // Sample format to send twilio message.
            //string AccountSid = "xxxxxxxxxxxxxxxxxxxxx";
            //string AuthToken = "xxxxxxxxxxxxxxxxxxxxxx";
            //var twilio = new TwilioRestClient(AccountSid, AuthToken);
            //var message = twilio.SendMessage("+17144595176", "7144691491", "test");
        }

        private void SendSMSMessageTest(Reminder reminder, Contact contact)
        {
            string fromPhoneNumber = string.Format("+1{0}", Security.Decrypt(ConfigurationManager.AppSettings["FromPhoneNumber"]));
            string toPhoneNumber = Regex.Replace(contact.PhoneNumber, "[^0-9]", ""); ;
            string message = this.SMSMessageToSendTest(reminder, contact);
            string image = "http://1.bp.blogspot.com/-CX0lw-6UhBc/U1EKIFJN0nI/AAAAAAAABAQ/IgTh46-7Jpo/s1600/Reminder2.png";

            string AccountSid = Security.Decrypt(ConfigurationManager.AppSettings["SMSAccountSid"]);
            string AuthToken = Security.Decrypt(ConfigurationManager.AppSettings["SMSAuthToken"]);
            

            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            if (string.IsNullOrEmpty(image))
            {
                twilio.SendMessage(fromPhoneNumber, toPhoneNumber, message);
            }
            else
            {
                twilio.SendMessage(fromPhoneNumber, toPhoneNumber, message, new string[] { image });
            }

            // Sample format to send twilio message.
            //string AccountSid = "xxxxxxxxxxxxxxxxxxxxx";
            //string AuthToken = "xxxxxxxxxxxxxxxxxxxxxx";
            //var twilio = new TwilioRestClient(AccountSid, AuthToken);
            //var message = twilio.SendMessage("+17144595176", "7144691491", "test");
        }

        private void RecordReminderSent(Reminder reminder)
        {
            _db.Reminders.ToList().Find(r => r.Id == reminder.Id).Sent = true;
            _db.Save();
        }

        private void RecordSendEmailHistory(Reminder reminder, Contact contact, Profile profile, DateTime serverCurrentDateTime)
        {
            var reminderHistory = new ReminderHistory();
            reminderHistory.ContactId = contact.Id;
            reminderHistory.Message = reminder.Message;
            reminderHistory.ProfileId = profile.Id;
            reminderHistory.ReminderDateTime = reminder.ReminderDateTime;
            reminderHistory.ReminderId = reminder.Id;
            reminderHistory.EmailSent = true;
            reminderHistory.SMSSent = false;
            reminderHistory.MessageSentDateTime = serverCurrentDateTime;
            _db.ReminderHistories.Add(reminderHistory);
            _db.Save();
        }

        private void RecordSendEmailHistoryTest(Reminder reminder, Contact contact, DateTime serverCurrentDateTime)
        {
            var reminderHistory = new ReminderHistory();
            reminderHistory.ContactId = contact.Id;
            reminderHistory.Message = reminder.Message;
            // reminderHistory.ProfileId = profile.Id;
            reminderHistory.ReminderDateTime = reminder.ReminderDateTime;
            reminderHistory.ReminderId = reminder.Id;
            reminderHistory.EmailSent = true;
            reminderHistory.SMSSent = false;
            reminderHistory.MessageSentDateTime = serverCurrentDateTime;
            _db.ReminderHistories.Add(reminderHistory);
            _db.Save();
        }

        private void RecordSendSMSHistory(Reminder reminder, Contact contact, Profile profile, DateTime serverCurrentDateTime)
        {
            var reminderHistory = new ReminderHistory();
            reminderHistory.ContactId = contact.Id;
            reminderHistory.Message = reminder.Message;
            reminderHistory.ProfileId = profile.Id;
            reminderHistory.ReminderDateTime = reminder.ReminderDateTime;
            reminderHistory.ReminderId = reminder.Id;
            reminderHistory.EmailSent = false;
            reminderHistory.SMSSent = true;
            reminderHistory.MessageSentDateTime = serverCurrentDateTime;
            _db.ReminderHistories.Add(reminderHistory);
            _db.Save();
        }

        private void RecordSendSMSHistoryTest(Reminder reminder, Contact contact, DateTime serverCurrentDateTime)
        {
            var reminderHistory = new ReminderHistory();
            reminderHistory.ContactId = contact.Id;
            reminderHistory.Message = reminder.Message;
            // reminderHistory.ProfileId = profile.Id;
            reminderHistory.ReminderDateTime = reminder.ReminderDateTime;
            reminderHistory.ReminderId = reminder.Id;
            reminderHistory.EmailSent = false;
            reminderHistory.SMSSent = true;
            reminderHistory.MessageSentDateTime = serverCurrentDateTime;
            _db.ReminderHistories.Add(reminderHistory);
            _db.Save();
        }

        private bool SendReminderAllowed(Reminder reminder)
        {
            var profile = _db.Profiles.ToList().Find(p => p.Id == reminder.ProfileId);
            var contact = new ReminderDb().Contacts.Where(c => c.Id == reminder.ContactId).FirstOrDefault();
            if (contact.Active && !profile.DeActivate)
            {
                return true;
            }

            return false;
        }

        private DateTime ServerCurrentDateTime(Reminder reminder)
        {
            DateTime currentDateTime;
            var contact = new ReminderDb().Contacts.Where(c => c.Id == reminder.ContactId).FirstOrDefault();
#if DEBUG
            currentDateTime = DateTime.Now;
#else
                int prodServerTimeDifference = 0;

				switch (contact.TimeZone)
				{
					case "PST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["PSTOffSetHours"]); break;
					case "MST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["MSTOffSetHours"]); break;
					case "CST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["CSTOffSetHours"]); break;
					case "EST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["ESTOffSetHours"]); break;
				}
				currentDateTime = DateTime.Now.AddHours(prodServerTimeDifference);
#endif

            return currentDateTime;
        }

        public void SendEmail(Reminder reminder, Profile profile, Contact contact)
        {
            string fromEmailAddress = profile.EmailAddress;
            string toEmailAddress = contact.EmailAddress;
            string emailSubject = string.Format("Reminder from {0} {1} - {2}", profile.FirstName, profile.LastName, DateTime.Now.ToString());
            string emailBody = string.Format("Hi {0}, <br/> This is a reminder for you to {1} at {2}. <br/> Sincerely,<br/> {3}", contact.FirstName.Trim(), reminder.Message, reminder.ReminderDateTime.DayOfWeek + " " + reminder.ReminderDateTime.ToString(), profile.FirstName);

            var emailMessage = new MessageEmail();
            emailMessage.Send(fromEmailAddress, toEmailAddress, emailSubject, emailBody, profile.FirstName + " " + profile.LastName, contact.FirstName + " " + contact.LastName);
        }

        public void SendSMS(Reminder reminder, Profile profile, Contact contact)
        {
            string fromPhoneNumber = profile.PhoneNumberIssued;
            string toPhoneNumber = string.Format("1{0}", contact.PhoneNumber);
            string message = string.Format("Hi {0}, This is a reminder for you to {1} at {2}. Sincerely, {3}", contact.FirstName.Trim(), reminder.Message, reminder.ReminderDateTime.ToString(), profile.FirstName);

            string AccountSid = profile.AccountSid;
            string AuthToken = profile.AuthToken;

            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            var messageSent = twilio.SendSmsMessage(fromPhoneNumber, toPhoneNumber, message, "");
        }

        public string SendTestEmail()
        {
            string msg = string.Empty;
            try
            {
                string fromEmailAddress = Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountAddress"]); // "myemail@mydomain.com";
                string toEmailAddress = "datagig@gmail.com";
                string emailSubject = "test subject";
                string emailBody = "Please confirm your account by clicking <a href=\"" + "http://www.microsoft.com" + "\">here</a>"; // "<b>test email body</b>"; 

                var emailMessage = new MessageEmail();
                
                emailMessage.Send(fromEmailAddress, toEmailAddress, emailSubject, emailBody, "Zulfiqar Syed", "Faisal Syed");
                return "successful send test email." + DateTime.Now.ToString();
            }
            catch(Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }

        }

        public string SendTestSMS()
        {
            try
            {
                string AccountSid = "AC36241612702f6674342ac88458c378c8";
                string AuthToken = Security.Decrypt("524c5c21a8/434`28/`a4b564b501b43");

                var twilio = new TwilioRestClient(AccountSid, AuthToken);

                var messageSent = twilio.SendSmsMessage("7144595176", "7144691491", "just a test sms message", "");
                return "successful send test sms." + DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }
        }

        public string Encrypt(string text)
        {
            return Security.Encrypt(text);
        }

        public string AddEmailAddressToUnsubscribeList(string EmailAddress)
        {
            // Create a form encoded string for the request body

            string parameters = "api_user=" + Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountAddress"]) + "&api_key=" + Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountPassword"]) + "&email=" + EmailAddress;

            //Create Request
            HttpWebRequest myHttpWebRequest = (HttpWebRequest) HttpWebRequest.Create("https://sendgrid.com/api/unsubscribes.add.json");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";

            // Create a new write stream for the POST body
            StreamWriter streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream());

            // Write the parameters to the stream
            streamWriter.Write(parameters);
            streamWriter.Flush();
            streamWriter.Close();

            // Get the response
            HttpWebResponse httpResponse = (HttpWebResponse) myHttpWebRequest.GetResponse();

            // Create a new read stream for the response body and read it
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
            string result = streamReader.ReadToEnd();
            // return result;
            result= string.Format("Email address {0} has been successfully unsubscribed.", EmailAddress);
            return JsonConvert.SerializeObject(result);
        }
    }
}
