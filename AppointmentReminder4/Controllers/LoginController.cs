using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointmentReminder4.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            var url = "http://" + Request.Url.Authority + "/#/Login";
            return Redirect(url);
        }
    }
}