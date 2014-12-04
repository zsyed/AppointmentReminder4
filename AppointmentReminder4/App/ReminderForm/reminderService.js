appointmentReminderApp.factory('reminderService',
	function ($http) {

		var getReminder = function (id) {
			return $http.get("/api/Reminder/" + id);
		};
		
		var getReminderForUpdate = function (id) {
			return $http.get("/api/Reminder/" + id);
		};

		var getReminders = function () {
			return $http.get("/api/Reminder");
		};
		
		var getReminderForUpdateContacts = function () {
			return $http.get("/api/ReminderContact");
		};

		var getReminderContacts = function () {
			return $http.get("/api/ReminderContact");
		};


		var insertReminder = function (reminder) {
			return $http.post("/api/Reminder", reminder);
		};

		var updateReminder = function (reminder) {
			return $http.put("/api/Reminder", reminder);
		};

		var deleteReminder = function (id) {
			return $http.delete("/api/Reminder/" + id);
		};

		var getReminderHistories = function () {
			return $http.get("/api/ReminderHistory");
		};

		return {
			getReminderForUpdateContacts: getReminderForUpdateContacts,
			getReminderForUpdate: getReminderForUpdate,
			getReminderContacts: getReminderContacts,
			getReminderHistories: getReminderHistories,
			insertReminder: insertReminder,
			deleteReminder: deleteReminder,
			updateReminder: updateReminder,
			getReminder: getReminder,
			getReminders: getReminders
		};
	}
);


