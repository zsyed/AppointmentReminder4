using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointmentReminder4.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            var url = "http://" + Request.Url.Authority + "/#/Register";
            return Redirect(url);
        }
    }
}