appointmentReminderApp.controller('authLogoutController',
	function AuthLogoutFormController($scope, $window, $location, $routeParams, authService) {
	    var s = "sss";
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