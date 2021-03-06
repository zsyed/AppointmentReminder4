﻿using AppointmentReminder4.Common;
using System;

namespace AppointmentReminder.Data
{
	public class Reminder
	{
		public virtual int Id { get; set; }

		public virtual int ProfileId { get; set; }

		public virtual DateTime ReminderDateTime { get; set; }

        public virtual Frequency Recurrence { get; set; } // once, daily, weekly, monthly

		public virtual string WeekDay { get; set; } // monday, tuesday, etc.

		public virtual string Message { get; set; }

        public virtual string EmailSubject { get; set; }

        public virtual string Image { get; set; }
		public virtual int ContactId { get; set; }

		public virtual bool Sent { get; set; }
	}
}