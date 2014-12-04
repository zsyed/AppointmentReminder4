appointmentReminderApp.controller('authLoginController',
	function AuthLoginFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.loginForm = function () {
	        authService.loginUser($scope.loginData).then(
                function (results) {
                    $location.path('/home');
                },
                    function (results) {
                        // on error
                        var data = results.data;
                    }
                );
	    };
	});