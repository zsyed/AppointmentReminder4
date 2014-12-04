appointmentReminderApp.factory('contactService',
	function ($http) {
		
		var getContact = function (id) {
			return $http.get("/api/Contact/" + id);
		};

		var getContacts = function() {
			return $http.get("/api/Contact");
		};

		var insertContact = function (contact) {
			return $http.post("/api/Contact", contact);
		};

		var updateContact = function (contact) {
			return $http.put("/api/Contact", contact);
		};
		
		var deleteContact = function (id) {
			return $http.delete("/api/Contact/" + id);
		};

		return {
			insertContact: insertContact,
			deleteContact: deleteContact,
			updateContact: updateContact,
			getContact: getContact,
			getContacts : getContacts
		};
	}
);


