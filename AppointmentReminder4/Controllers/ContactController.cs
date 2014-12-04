using AppointmentReminder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentReminder4.Controllers
{
    public class ContactController : ApiController
    {
		private IReminderDb _db;

		public ContactController(IReminderDb db)
		{
			_db = db;
		}

		public IQueryable<Contact> GetAllContacts()
		{
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
			var contacts = new ReminderDb().Contacts.Where(c => c.ProfileId == profile.Id).OrderBy(c => c.LastName);
			return contacts;
		}

		public Contact Get(int Id)
		{
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
			var contact = new ReminderDb().Contacts.Where(c => c.ProfileId == profile.Id).ToList().Find(c => c.Id == Id);
			return contact;
		}

		public HttpResponseMessage Put([FromBody] Contact contact)
		{
			try
			{
				var dbContact = _db.Contacts.ToList().Find(c => c.Id == contact.Id);
				dbContact.FirstName = contact.FirstName;
				dbContact.LastName = contact.LastName;
				dbContact.EmailAddress = contact.EmailAddress;
				dbContact.PhoneNumber = contact.PhoneNumber;
				dbContact.Active = contact.Active;
				dbContact.SendEmail = contact.SendEmail;
				dbContact.SendSMS = contact.SendSMS;
				dbContact.TimeZone = contact.TimeZone;
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.Created, contact);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		public HttpResponseMessage Delete(int Id)
		{
			var dbContact = _db.Contacts.ToList().Find(c => c.Id == Id);

			if (dbContact == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			try
			{
				_db.Contacts.Remove(dbContact);
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.OK);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		[HttpPost]
		[ActionName("Contact")]
		public HttpResponseMessage Post([FromBody]Contact contact)
		{
			try
			{
				var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
				contact.ProfileId = profile.Id;
				contact.Active = true;
				_db.Contacts.Add(contact);
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.Created, contact);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}
	}
}