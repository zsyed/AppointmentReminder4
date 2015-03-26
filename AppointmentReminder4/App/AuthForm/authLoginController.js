appointmentReminderApp.controller('authLoginController',
	function AuthLoginFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.message = "";
	    $scope.savedSuccessfully = false;

	    $scope.loadinglogin = false;
	    $scope.loginLoaded = false;

	    $scope.$watch('loginLoaded', function (value) {
	        $scope.loadinglogin = value;
	    });

	    $scope.loginForm = function () {
	        $scope.loginLoaded = true;
	        authService.loginUser($scope.loginData).then(
                function (results) {
                    $scope.loginLoaded = false;
                    //authService.checkProfile();
                    //authService.checkContact();

                    $location.path('/home');
                },
                    function (results) {
                        // on error
                        // var data = results.data;
                        $scope.loginLoaded = false;
                        $scope.savedSuccessfully = false;
                        $scope.message = "Login attempt failed. Email address and/or password is incorrect."; // results.error_description;
                    }
                );
	    };

	    $scope.resetPasswordRedirect = function () {
	        $location.path('/ResetPassword');
	    };

	});