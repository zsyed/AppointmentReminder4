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
	        $scope.successMsg = "You should be getting your test SMS message and email within 90 seconds.";
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