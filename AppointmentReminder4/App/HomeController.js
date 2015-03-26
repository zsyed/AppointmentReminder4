appointmentReminderApp.controller('HomeController',
	function HomeFormController($scope, $window, $location, $routeParams, reminderService) {

	    $scope.errorMsg = "";
	    $scope.successMsg = "";

	    $scope.tryingtoload = false;
	    $scope.loading = false;
	    $scope.randomNumber = randomString(4);
	    $scope.random = "";

	    $scope.validateTextbox = function () {
	        return $scope.randomNumber;
	    };

	    $scope.$watch('tryingtoload', function (value) {
	        $scope.loading = value;
	    });

	    var onReminderTestComplete = function (data) {
	        $scope.tryingtoload = false;
	        $scope.successMsg = "You should be getting your test SMS message and email within 90 seconds.";
	        $scope.reminderTest.PhoneNumber = "";
	        $scope.reminderTest.EmailAddress = "";
	        $scope.reminderTest.random = "";
	        $scope.randomNumber = randomString(4);

	        SetFormPristing();
	    };

	    var onReminderErrorTest = function (reason) {
	        $scope.tryingtoload = false;
	        $scope.error = "oops some thing went wrong.You may not get your test message.";
	        $scope.errorMsg = "oops some thing went wrong.You may not get your test message.";

	        SetFormPristing();
	    };

	    $scope.submitTestReminderForm = function () {
	        $scope.errorMsg = "";
	        $scope.tryingtoload = true;
	        if ($scope.reminderTest.random == $scope.randomNumber) {
	            reminderService.insertTestReminder($scope.reminderTest).then(onReminderTestComplete, onReminderErrorTest);
	        }
	        else {
	            $scope.tryingtoload = false;
	            $scope.errorMsg = "The Captcha values you entered " + $scope.reminderTest.random + " did not match with actual Captcha " + $scope.randomNumber + ". Please retry";
	        }
	    };


	    //$scope.randomNumberPattern = function () {
	    //    return "/3333/";
	    //};

	    function randomString(len, an) {
	        an = an && an.toLowerCase();
	        var str = "", i = 0, min = an == "a" ? 10 : 0, max = an == "n" ? 10 : 62;
	        for (; i++ < len;) {
	            var r = Math.random() * (max - min) + min << 0;
	            str += String.fromCharCode(r += r > 9 ? r < 36 ? 55 : 61 : 48);
	        }
	        return str;
	    }

	    function SetFormPristing() {
	        $scope.testMessageForm.PhoneNumber.$invalid = false;
	        $scope.testMessageForm.PhoneNumber.$dirty = false;

	        $scope.testMessageForm.EmailAddress.$invalid = false;
	        $scope.testMessageForm.EmailAddress.$dirty = false;

	        $scope.testMessageForm.random.$invalid = false;
	        $scope.testMessageForm.random.$dirty = false;
	    }
	});