appointmentReminderApp.controller('authLoginController',
	function AuthLoginFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.loadinglogin = false;
	    $scope.loginForm = function () {
	        $scope.$broadcast('LOGIN_LOADED_EVENT');
	        authService.loginUser($scope.loginData).then(
                function (results) {

                    $location.path('/home');
                },
                    function (results) {
                        // on error
                        var data = results.data;
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