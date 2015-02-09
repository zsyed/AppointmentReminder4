using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointmentReminder4.Controllers
{
    public class ResetPasswordController : Controller
    {
        // GET: ResetPassword
        public ActionResult Index()
        {
            var url = "http://" + Request.Url.Authority + "/#/ResetPassword";
            return Redirect(url);
        }
    }
}