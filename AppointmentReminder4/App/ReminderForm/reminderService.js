appointmentReminderApp.factory('reminderService',
	function ($http) {

		var getReminder = function (id) {
		    return $http.get("/api/Reminder/" + id).then(function (response) { return response.data; });
		};
		
		var getReminderForUpdate = function (id) {
		    return $http.get("/api/Reminder/" + id).then(function (response) { return response.data; });
		};

		var getReminders = function () {
		    return $http.get("/api/Reminder").then(function (response) { return response.data; });
		};
		
		var getReminderForUpdateContacts = function () {
		    return $http.get("/api/ReminderContact").then(function (response) { return response.data; });
		};

		var getReminderContacts = function () {
		    return $http.get("/api/ReminderContact").then(function (response) { return response.data; });
		};

		var insertReminder = function (reminder) {
		    return $http.post("/api/Reminder", reminder).then(function (response) { return response.data; });
		};

		var updateReminder = function (reminder) {
		    return $http.put("/api/Reminder", reminder).then(function (response) { return response.data; });
		};

		var deleteReminder = function (id) {
		    return $http.delete("/api/Reminder/" + id).then(function (response) { return response.data; });
		};

		var getReminderHistories = function () {
		    return $http.get("/api/ReminderHistory").then(function (response) { return response.data; });
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


