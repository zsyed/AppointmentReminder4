﻿appointmentReminderApp.factory('contactService',
	function ($http) {
		
		var getContact = function (id) {
		    return $http.get("/api/Contact/" + id).then(function (response) { return response.data; });
		};

		var getContacts = function() {
		    return $http.get("/api/Contact").then(function (response) { return response.data; });
		};

		var insertContact = function (contact) {
		    return $http.post("/api/Contact", contact).then(function (response) { return response.data; });
		};

		var updateContact = function (contact) {
		    return $http.put("/api/Contact", contact).then(function (response) { return response.data; });
		};
		
		var deleteContact = function (id) {
		    return $http.delete("/api/Contact/" + id).then(function (response) { return response.data; });
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


