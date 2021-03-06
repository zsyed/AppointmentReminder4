﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentReminder.Models
{
    using AppointmentReminder4.Common;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

	public class ReminderModel
	{
		[HiddenInput(DisplayValue = false)]
		public int Id { get; set; }

		[HiddenInput(DisplayValue = false)]
		public int ProfileId { get; set; }

		[Required]
		public DateTime ReminderDateTime { get; set; }

		[Required]
		public string Message { get; set; }

        public string Image { get; set; }

		[Required]
		public int ContactId { get; set; }

		public string ContactName { get; set; }

		public Frequency Recurrence { get; set; }

		public string WeekDay { get; set; }

		public bool Sent { get; set; }

        [Required]
        public string EmailSubject { get; set; }
	}
}