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
                var reminder = new Reminder();
                reminder.ProfileId = 0;
                reminder.EmailSubject = "Test Email Subject";
                reminder.Message = "This is a Test Message";
                // reminder.Image = "";
                reminder.ReminderDateTime = DateTime.Now.AddMinutes(3);
                reminder.Recurrence = "Once";
                // reminder.WeekDay = reminderModel.WeekDay;
                // reminder.Sent = reminderModel.Sent;
                _db.Reminders.Add(reminder);
                _db.Save();
                // return Request.CreateResponse(HttpStatusCode.Created, reminder);

                contact.FirstName = "Test First";
                contact.LastName = "Test Last";
                contact.ProfileId = 0;
                contact.SendEmail = true;
                contact.SendSMS = true;
                contact.Active = true;
                _db.Contacts.Add(contact);
                _db.Save();

                return Request.CreateResponse(HttpStatusCode.Created, contact);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
