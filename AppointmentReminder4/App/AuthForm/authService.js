//appointmentReminderApp.factory('authService',
//	function ($http,$q) {

appointmentReminderApp.factory('authService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

        var serviceBase = "/";

        var getValues = function () {

            return $http.get(serviceBase + 'api/values').then(function (results) {
                return results;
            });
        };

        var _authentication = {
            isAuth: false,
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

	    var logoutUser = function () {

	        localStorageService.remove('authorizationData');

	        _authentication.isAuth = false;
	        _authentication.userName = "";
	        _authentication.useRefreshTokens = false;

	    };

	    var loginUser = function (loginData) {

	        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.Password;

	        //if (loginData.useRefreshTokens) {
	        //    data = data + "&client_id=" + ngAuthSettings.clientId;
	        //}

	        var deferred = $q.defer();

	        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

	            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });


	            //if (loginData.useRefreshTokens) {
	            //    localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
	            //}
	            //else {
	            //    localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
	            //}

	            _authentication.isAuth = true;
	            _authentication.userName = loginData.userName;
	            _authentication.useRefreshTokens = loginData.useRefreshTokens;

	            response.authentication = _authentication;
	            deferred.resolve(response);

	        }).error(function (err, status) {
	            alert(err.error_description);
	            logoutUser();
	            deferred.reject(err);
	        });

	        return deferred.promise;

	    };


	    return {
            registerUser: registerUser,
            loginUser: loginUser,
            logoutUser: logoutUser,
            authentication: _authentication,
            getValues: getValues
	    };
	}
]);