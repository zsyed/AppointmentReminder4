appointmentReminderApp.controller('authRegisterController',
	function AuthFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.registerForm = function () {
	        authService.registerUser($scope.auth).then(
                function (results) {
                    // $scope.auth = results.data;
                    // $window.history.back();
                    $location.path('/RegisterSuccess');
                },
                    function (results) {
                        // on error
                        var data = results.data;
                    }
                );
	    };
	});