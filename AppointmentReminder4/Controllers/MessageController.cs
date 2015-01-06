using AppointmentReminder.Data;
using AppointmentReminder4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;

namespace AppointmentReminder4.Controllers
{ 
    public class MessageController : Controller
    {
        //
        // GET: /Message/ 

        private IReminderDb _db;

        public MessageController(IReminderDb db)
        {
            _db = db;
        }

        public string CurrentDateTimeValue(string TimeZone)
        {
            int prodServerTimeDifference = 0;

            switch (TimeZone)
            {
                case "PST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["PSTOffSetHours"]); break;
                case "MST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["MSTOffSetHours"]); break;
                case "CST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["CSTOffSetHours"]); break;
                case "EST": prodServerTimeDifference = Convert.ToInt32(ConfigurationManager.AppSettings["ESTOffSetHours"]); break;
            }

            return DateTime.Now.AddHours(prodServerTimeDifference).ToString();
        }

        public string CurrentDateTime()
        {
            return DateTime.Now.ToString();
        }

        public JsonResult SendReminder()
        {
            // For each reminder
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
            if (reminder.Recurrence == "Once")
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

        private bool SendDaily(Contact contact, Reminder reminder, DateTime serverCurrentDateTime)
        {
            bool reminderSent = false;
            if (reminder.Recurrence == "Daily")
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
            if (reminder.Recurrence == "Weekly")
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

        private string EmailMessageToSend(Reminder reminder, Profile profile, Contact contact)
        {
            return MessageToSend(reminder, profile, contact);
        }

        private string SMSMessageToSend(Reminder reminder, Profile profile, Contact contact)
        {
            return MessageToSend(reminder, profile, contact);
        }

        private string MessageToSend(Reminder reminder, Profile profile, Contact contact)
        {
            return string.Format("Hi {0},{1}{2}{1}Sincerely,{1}{3}", contact.FirstName.Trim(), System.Environment.NewLine, reminder.Message, profile.FirstName);
        }


        private void SendEmailMessage(Reminder reminder, Profile profile, Contact contact)
        {
            string fromEmailAddress = profile.EmailAddress;
            string toEmailAddress = contact.EmailAddress;
            string emailSubject = string.Format("Reminder from {0} {1} - {2}", profile.FirstName, profile.LastName, DateTime.Now.ToString());
            string emailBody = this.EmailMessageToSend(reminder, profile, contact);
            var emailMessage = new MessageEmail();
            emailMessage.Send(fromEmailAddress, toEmailAddress, emailSubject, emailBody, profile.FirstName + " " + profile.LastName, contact.FirstName + " " + contact.LastName);
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
                string fromEmailAddress = "azure_a517861c408b1278d9304b663d4fca14@azure.com"; // "myemail@mydomain.com";
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


    }
}
