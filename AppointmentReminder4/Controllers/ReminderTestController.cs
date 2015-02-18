using AppointmentReminder.Data;
using AppointmentReminder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentReminder4.Controllers
{
    public class ReminderTestController : ApiController
    {
        private IReminderDb _db;

		public ReminderTestController(IReminderDb db)
		{
			_db = db;
		}

        [HttpPost]
        [ActionName("Reminder")]
        public HttpResponseMessage Post([FromBody]Contact contact)
        {
            try
            {
                var profile = new Profile();
                profile.EmailAddress = "Test Email Address";
                profile.FirstName = Guid.NewGuid().ToString();
                profile.LastName = "Test";
                profile.DeActivate = false;
                _db.Profiles.Add(profile);
                _db.Save();
                var profileSaved = new ReminderDb().Profiles.Where(p => p.FirstName == profile.FirstName).FirstOrDefault();

                if (profileSaved != null)
                {
                    var reminder = new Reminder();
                    reminder.ProfileId = profileSaved.Id;
                    reminder.EmailSubject = "Thank you for testing.";
                    reminder.Message = "This is a test message generated when you entered your cell phone number and email address on our home page. Please register with us to get started with getting useful reminders and take your business to next level.";
                    reminder.ReminderDateTime = DateTime.Now.AddMinutes(3);
                    reminder.Recurrence = "Once";

                    _db.Reminders.Add(reminder);
                    _db.Save();

                    contact.FirstName = "Guest";
                    contact.LastName = "User";
                    contact.ProfileId = profileSaved.Id;
                    contact.SendEmail = true;
                    contact.SendSMS = true;
                    contact.Active = true;
                    _db.Contacts.Add(contact);
                    _db.Save();

                    return Request.CreateResponse(HttpStatusCode.Created, contact);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
