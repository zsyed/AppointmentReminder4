appointmentReminderApp.controller('authLogoutController',
	function AuthLogoutFormController($scope, $window, $location, $routeParams, authService) {
	        authService.logoutUser().then(
                function (results) {
                    //$scope.auth = results.data;
                    $location.path('/home');
                },
                    function (results) {
                        // on error
                        var data = results.data;
                    }
                );

	});