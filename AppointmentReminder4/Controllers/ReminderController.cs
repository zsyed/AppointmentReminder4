﻿using AppointmentReminder.Data;
using AppointmentReminder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentReminder4.Controllers
{
    public class ReminderController : ApiController
	{
		private IReminderDb _db;

		public ReminderController(IReminderDb db)
		{
			_db = db;
		}

		public HttpResponseMessage Put([FromBody] ReminderModel reminder)
		{
			try
			{
				var dbReminder = _db.Reminders.ToList().Find(r => r.Id == reminder.Id);
				dbReminder.ContactId = reminder.ContactId;
                dbReminder.EmailSubject = reminder.EmailSubject;
				dbReminder.Message = reminder.Message;
				dbReminder.ProfileId = reminder.ProfileId;
				dbReminder.ReminderDateTime = reminder.ReminderDateTime;
				dbReminder.Sent = reminder.Sent;
				dbReminder.Recurrence = reminder.Recurrence;
                dbReminder.Image = reminder.Image;
				dbReminder.WeekDay = reminder.WeekDay;
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.Created, reminder);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		public List<ReminderModel> GetAllReminders()
		{
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
            if (profile != null)
            {
                var reminders = new ReminderDb().Reminders.Where(r => r.ProfileId == profile.Id).OrderByDescending(r => r.ReminderDateTime);

                if (reminders.Count() > 0)
                {
                    var remindersModel = new List<ReminderModel>();
                    foreach (var reminder in reminders)
                    {
                        var contact = _db.Contacts.ToList().Find(c => c.Id == reminder.ContactId);
                        if (contact != null)
                        {
                            remindersModel.Add(new ReminderModel()
                                {
                                    Id = reminder.Id,
                                    ContactId = reminder.ContactId,
                                    EmailSubject = reminder.EmailSubject,
                                    Message = reminder.Message,
                                    ProfileId = reminder.ProfileId,
                                    ReminderDateTime = reminder.ReminderDateTime,
                                    ContactName = string.Format("{0} {1}", contact.FirstName, contact.LastName),
                                    Recurrence = reminder.Recurrence,
                                    WeekDay = reminder.WeekDay,
                                    Sent = reminder.Sent
                                }
                            );
                        }
                    }
                    return remindersModel;
                }
            }

            return null;
		}

		public ReminderModel Get(int Id)
		{
            string message = string.Empty;
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
            if (profile != null)
            {
                var reminder = _db.Reminders.Where(p => p.ProfileId == profile.Id).ToList().Find(r => r.Id == Id);
                if (reminder != null)
                {
                    var contact = _db.Contacts.ToList().Find(c => c.Id == reminder.ContactId);
                    if (contact != null)
                    {
                        var reminderModel = new ReminderModel()
                                                {
                                                    Id = reminder.Id,
                                                    ContactId = reminder.ContactId,
                                                    EmailSubject = reminder.EmailSubject,
                                                    Message = reminder.Message,
                                                    ProfileId = reminder.ProfileId,
                                                    ReminderDateTime = reminder.ReminderDateTime,
                                                    ContactName = string.Format("{0} {1}", contact.FirstName, contact.LastName),
                                                    Recurrence = reminder.Recurrence,
                                                    WeekDay = reminder.WeekDay,
                                                    Image = reminder.Image,
                                                    Sent = reminder.Sent
                                                };
                        return reminderModel;
                    }
                }
            }
            return null;            
		}

		public HttpResponseMessage Delete(int Id)
		{
			var dbReminder = _db.Reminders.ToList().Find(r => r.Id == Id);

			if (dbReminder == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			try
			{
				_db.Reminders.Remove(dbReminder);
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.OK);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		[HttpPost]
		[ActionName("Reminder")]
		public HttpResponseMessage Post([FromBody]ReminderModel reminderModel)
		{
			try
			{
				var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
				var reminder = new Reminder();
				reminder.ProfileId = profile.Id;
				reminder.ContactId = reminderModel.ContactId;
                reminder.EmailSubject = reminderModel.EmailSubject;
				reminder.Message = reminderModel.Message;
                reminder.Image = reminderModel.Image;
				reminder.ReminderDateTime = reminderModel.ReminderDateTime;
				reminder.Recurrence = reminderModel.Recurrence;
				reminder.WeekDay = reminderModel.WeekDay;
				reminder.Sent = reminderModel.Sent;
				_db.Reminders.Add(reminder);
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.Created, reminder);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

	}
}