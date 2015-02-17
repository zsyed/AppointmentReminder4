appointmentReminderApp.controller('HomeController',
	function HomeFormController($scope, $window, $location, $routeParams, reminderService) {

	    var onReminderTestComplete = function (data) {
	        $scope.reminderTest = data;
	    };

	    var onReminderErrorTest = function (reason) {
	        $scope.error = "Could not send test reminder.";
	    };

	    $scope.submitTestReminderForm = function () {
	        reminderService.insertTestReminder($scope.reminderTest).then(onReminderTestComplete, onReminderErrorTest);
	    };
	});