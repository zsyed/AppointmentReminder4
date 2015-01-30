appointmentReminderApp.factory('profileService',
	function ($http) {
		var getProfile = function () {
		    return $http.get("/api/Profile").then(function (response) { return response.data; });		                
		};
		
		var insertProfile = function (profile) {
		    return $http.post("/api/Profile", profile).then(function (response) { return response.data; });
		};

		var updateProfile = function (profile) {
			
		    return $http.put("/api/Profile", profile).then(function (response) { return response.data; });
		};

		return {
			insertProfile: insertProfile,
			updateProfile: updateProfile,
			getProfile: getProfile
		};
	}
);
