appointmentReminderApp.controller('authLoginController',
	function AuthLoginFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.message = "";
	    $scope.savedSuccessfully = false;
	    $scope.loadinglogin = false;
	    $scope.loginForm = function () {
	        $scope.$broadcast('LOGIN_LOADED_EVENT');
	        authService.loginUser($scope.loginData).then(
                function (results) {

                    authService.checkProfile();
                    authService.checkContact();

                    $location.path('/home');
                },
                    function (results) {
                        // on error
                        // var data = results.data;
                        $scope.savedSuccessfully = false;
                        $scope.message = "Login attempt failed. Email address and/or password is incorrect."; // results.error_description;
                        $scope.$broadcast('LOGIN_STOPLOADED_EVENT');
                    }
                );
	    };

	    $scope.$on('LOGIN_LOADED_EVENT', function () {
	        $scope.loadinglogin = true;
	    });

	    $scope.$on('LOGIN_STOPLOADED_EVENT', function () {
	        $scope.loadinglogin = false;
	    });

	    $scope.resetPassword = function () {
	        $location.path('/ResetPassword');
	    };
	});