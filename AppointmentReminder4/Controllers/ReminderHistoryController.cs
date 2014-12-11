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
    public class ReminderHistoryController : ApiController
    {
		private IReminderDb _db;

		public ReminderHistoryController(IReminderDb db)
		{
			_db = db;
		}

		public List<ReminderHistoryModel> Get()
		{
			try
			{
				var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
                if (profile != null)
                {
                    var reminderHistories = _db.ReminderHistories.Where(rh => rh.ProfileId == profile.Id).OrderByDescending(rh => rh.MessageSentDateTime);
                    var reminderHistoryModels = new List<ReminderHistoryModel>();
                    foreach (var reminderHistory in reminderHistories)
                    {
                        var contact = new ReminderDb().Contacts.ToList().Find(c => c.Id == reminderHistory.ContactId);
                        if (contact != null)
                        {
                            var reminder = new ReminderDb().Reminders.ToList().Find(r => r.Id == reminderHistory.ReminderId);
                            if (reminder != null && contact != null)
                            {
                                reminderHistoryModels.Add(
                                    new ReminderHistoryModel()
                                        {
                                            Message = reminderHistory.Message,
                                            ReminderDateTime = reminderHistory.ReminderDateTime,
                                            ContactName = string.Format("{0} {1}", contact.FirstName, contact.LastName),
                                            SMSSent = reminderHistory.SMSSent,
                                            EmailSent = reminderHistory.EmailSent,
                                            MessageSentDateTime = reminderHistory.MessageSentDateTime,
                                            Recurrence = reminder.Recurrence,
                                            Weekday = reminder.WeekDay
                                        });
                            }
                        }
                    }
                    return reminderHistoryModels;
                }

                return null;
			}
			catch (Exception exception)
			{
				throw exception;
			}

		}
	}
}