using AppointmentReminder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentReminder4.Controllers
{
    public class ReminderContactController : ApiController
    {
 		private IReminderDb _db;
		public ReminderContactController(IReminderDb db)
		{
			_db = db;
		}
		public IHttpActionResult GetAllReminderContacts()
		{
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
			var profileContacts = _db.Contacts.Where(c => c.ProfileId == profile.Id).ToList();
			var contacts = new List<ReminderContact>();
			foreach (var profileContact in profileContacts)
			{
				contacts.Add(new ReminderContact()
					             {
						             Id = profileContact.Id, 
									 name = string.Format("{0} {1}", profileContact.FirstName, profileContact.LastName)
					             });
			}
			return this.Ok(contacts);
		}


	}
}