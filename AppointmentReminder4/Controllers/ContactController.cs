using AppointmentReminder.Data;
using AppointmentReminder.Models;
using AppointmentReminder4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

		public List<ContactModel> GetAllContacts()
		{
            var unsubscribeEmailAddresses = EmailAddressesUnsubscibed();
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
            if (profile != null)
            {
                var contacts = new ReminderDb().Contacts.Where(c => c.ProfileId == profile.Id).OrderBy(c => c.LastName);
                if (contacts != null)
                {
                    var contactsModel = new List<ContactModel>();

                    foreach (var contact in contacts)
                    {
                        bool unsubscribe = unsubscribeEmailAddresses.Any(e => e.Equals(contact.EmailAddress));

                        contactsModel.Add(new ContactModel() { 
                            Id = contact.Id,
                            ProfileId = contact.ProfileId,
                            FirstName = contact.FirstName,
                            LastName = contact.LastName,
                            PhoneNumber = contact.PhoneNumber,
                            EmailAddress = contact.EmailAddress,
                            TimeZone = contact.TimeZone,
                            Active = contact.Active,
                            SendEmail = contact.SendEmail,
                            SendSMS = contact.SendSMS,
                            EmailUnsubscribe = unsubscribe
                        });
                    }
                    return contactsModel;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
		}

		public ContactModel Get(int Id)
		{
            var unsubscribeEmailAddresses = EmailAddressesUnsubscibed();
            
			var profile = _db.Profiles.ToList().Find(p => p.UserName == User.Identity.Name);
            if (profile != null)
            {
                var contact = new ReminderDb().Contacts.Where(c => c.ProfileId == profile.Id).ToList().Find(c => c.Id == Id);
                if (contact != null)
                {
                    bool unsubscribe = unsubscribeEmailAddresses.Any(e => e.Equals(contact.EmailAddress));
                    var contactModel = new ContactModel() { 
                                Id = contact.Id,
                                ProfileId = contact.ProfileId,
                                FirstName = contact.FirstName,
                                LastName = contact.LastName,
                                PhoneNumber = contact.PhoneNumber,
                                EmailAddress = contact.EmailAddress,
                                TimeZone = contact.TimeZone,
                                Active = contact.Active,
                                SendEmail = contact.SendEmail,
                                SendSMS = contact.SendSMS,
                                EmailUnsubscribe = unsubscribe
                            };

                    return contactModel;
                }
            }

            return null;
		}

		public HttpResponseMessage Put([FromBody] Contact contact)
		{
            try
			{
				var dbContact = _db.Contacts.ToList().Find(c => c.Id == contact.Id);
				dbContact.FirstName = contact.FirstName;
				dbContact.LastName = contact.LastName;
				dbContact.EmailAddress = contact.EmailAddress;
                dbContact.PhoneNumber = new String(contact.PhoneNumber.Where(Char.IsDigit).ToArray());
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
                contact.PhoneNumber = new String(contact.PhoneNumber.Where(Char.IsDigit).ToArray());
				contact.ProfileId = profile.Id;
                contact.SendEmail = true;
                contact.SendSMS = true;
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

        private List<string> EmailAddressesUnsubscibed()
        {
            string parameters = "api_user=" + Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountAddress"]) + "&api_key=" + Security.Decrypt(ConfigurationManager.AppSettings["EmailAccountPassword"]) + "&date=0";

            //Create Request
            string url = "https://api.sendgrid.com/api/unsubscribes.get.json" + "?" + parameters;
            var unsubscribeEmailAddresses = new List<string>();

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var emailAddress = line.Split('"')[3];
                    unsubscribeEmailAddresses.Add(emailAddress);
                }
            }

            return unsubscribeEmailAddresses;
        }
	}
}