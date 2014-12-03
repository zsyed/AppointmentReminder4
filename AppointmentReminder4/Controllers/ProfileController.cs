using AppointmentReminder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentReminder4.Controllers
{
    [Authorize]
    public class ProfileController : ApiController
    {
	    private IReminderDb _db;

		public ProfileController(IReminderDb db)
		{
			_db = db;
		}

		public Profile Get()
		{
			string userName = User.Identity.Name;
			var profile = _db.GetProfile(userName);
			return profile;
		}

		public HttpResponseMessage Put([FromBody] Profile profile)
		{
			try
			{
				var dbProfile = _db.Profiles.ToList().Find(p => p.Id == profile.Id);
				dbProfile.FirstName = profile.FirstName;
				dbProfile.LastName = profile.LastName;
				dbProfile.EmailAddress = profile.EmailAddress;
				dbProfile.PhoneNumber = profile.PhoneNumber;
				dbProfile.DeActivate = profile.DeActivate;
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.Created, profile);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		public HttpResponseMessage Post([FromBody]Profile newProfile)
		{
			try
			{
				newProfile.UserName = User.Identity.Name;
				newProfile.DeActivate = false;
				_db.Profiles.Add(newProfile);
				_db.Save();
				return Request.CreateResponse(HttpStatusCode.Created, newProfile);
			}
			catch (Exception)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}
    }
}
