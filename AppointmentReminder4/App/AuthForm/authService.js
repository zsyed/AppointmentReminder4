appointmentReminderApp.factory('authService', ['$http', '$q', 'localStorageService', 'profileService','contactService', function ($http, $q, localStorageService, profileService, contactService) {

        var serviceBase = "/";

        var getValues = function () {
            return $http.get(serviceBase + 'api/values').then(function (results) {
                return results;
            });
        };

        var _authentication = {
            isAuth: false,
            profileExists: false,
            contactExists : false,
            userName: "",
            useRefreshTokens: false
        };

        var _externalAuthData = {
            provider: "",
            userName: "",
            externalAccessToken: ""
        };

        var registerUser = function (auth) {
            logoutUser();
	        return $http.post("/api/Account/Register", auth);
        };

        var resetPassword = function (passwordResetData) {
            return $http.post("/api/Account/ResetPassword", passwordResetData).then(function (response) { return response.data; });
        };

	    var logoutUser = function () {
	        localStorageService.remove('authorizationData');
	        _authentication.isAuth = false;
	        _authentication.profileExists = false;
	        _authentication.contactExists = false;
	        _authentication.userName = "";
	        _authentication.useRefreshTokens = false;

	    };

	    var profileExists = function (exists)
	    {
	        _authentication.profileExists = exists;
	    }

	    var contactExists = function (exists) {
	        _authentication.contactExists = exists;
	    }

	    var loginUser = function (loginData) {

	        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.Password;

	        var deferred = $q.defer();

	        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

	            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });

	            _authentication.isAuth = true;
	            _authentication.userName = loginData.userName;
	            _authentication.useRefreshTokens = loginData.useRefreshTokens;

	            response.authentication = _authentication;
	            deferred.resolve(response);

	        }).error(function (err, status) {
	            logoutUser();
	            deferred.reject(err);
	        });
	        return deferred.promise;
	    };

	    return {
            registerUser: registerUser,
            loginUser: loginUser,
            //checkProfile: checkProfile,
            profileExists: profileExists,
            //checkContact: checkContact,
            contactExists: contactExists,
            logoutUser: logoutUser,
            authentication: _authentication,
            getValues: getValues,
            resetPassword: resetPassword
	    };
	}
]);