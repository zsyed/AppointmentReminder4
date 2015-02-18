appointmentReminderApp.controller('HomeController',
	function HomeFormController($scope, $window, $location, $routeParams, reminderService) {

	    $scope.errorMsg = "";
	    $scope.successMsg = "";

	    $scope.tryingtoload = false;
	    $scope.loading = false;

	    $scope.$watch('tryingtoload', function (value) {
	        $scope.loading = value;
	    });

	    var onReminderTestComplete = function (data) {
	        $scope.tryingtoload = false;
	        $scope.reminderTest = data;
	        $scope.successMsg = "You should be getting your test SMS message on " + data.data.PhoneNumber + " and " + data.data.EmailAddress + " within 90 seconds.";
	    };

	    var onReminderErrorTest = function (reason) {
	        $scope.tryingtoload = false;
	        $scope.error = "oops some thing went wrong.You may not get your test message.";
	        $scope.errorMsg = "oops some thing went wrong.You may not get your test message.";
	    };

	    $scope.submitTestReminderForm = function () {
	        $scope.tryingtoload = true;
	        reminderService.insertTestReminder($scope.reminderTest).then(onReminderTestComplete, onReminderErrorTest);
	    };
	});