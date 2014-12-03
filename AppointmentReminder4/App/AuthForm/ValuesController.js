appointmentReminderApp.controller('ValuesController',
	function ValuesFormController($scope, $window, $location, $routeParams, authService) {
	    var s = "temp";
	        authService.getValues().then(
                function (results) {
                    $scope.values = results.data;
                },
                    function (results) {
                        // on error
                        var data = results.data;
                    }
                );
	
	});