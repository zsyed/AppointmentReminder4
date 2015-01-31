appointmentReminderApp.controller('authRegisterController',
	function AuthFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.savedSuccessfully = false;
	    $scope.message = "";

	    $scope.loadingRegister = false;
	    $scope.registerLoaded = false;

	    $scope.$watch('registerLoaded', function (value) {
	        $scope.loadingRegister = value;
	    });

	    $scope.registerForm = function () {
	        $scope.registerLoaded = true;
	        authService.registerUser($scope.auth)
                .success(function (data, status, headers, config) {
                    $scope.registerLoaded = false;
                        $scope.message = '';
                        $scope.errors =[];
                        if (data.success === false) {

                            $scope.message = data.Message;
                            $scope.savedSuccessfully = false;

                        }
                        else {
                            $scope.savedSuccessfully = true;
                            $scope.message = 'Partial Registration Successful. Please click the link in the email that we just sent you to complete the registration. ';
                        }
                            })
                .error(function (data, status, headers, config) {
                    $scope.registerLoaded = false;
                    $scope.message = data.Message;
                    $scope.savedSuccessfully = false;
                });
        };

	    $scope.resetPassword = function () {
	        authService.resetPassword($scope.passwordResetData)
                .success(function (data, status, headers, config) {
                        $scope.message = '';
                        $scope.errors =[];
                        if (data.success === false) {

                            $scope.message = data.Message;
                            $scope.savedSuccessfully = false;

                        }
                        else {
                            $scope.savedSuccessfully = true;
                            $scope.message = 'Password reset was successful. Please click on the link in the email just sent to you to complete this process.';
                        }
                            })
                .error(function (data, status, headers, config) {
                    $scope.message = data.Message;
                    $scope.savedSuccessfully = false;
                });
	    };
	});

appointmentReminderApp.directive("passwordVerify", function () {
    return {
        require: "ngModel",
        scope: {
            passwordVerify: '='
        },
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(function () {
                var combined;

                if (scope.passwordVerify || ctrl.$viewValue) {
                    combined = scope.passwordVerify + '_' + ctrl.$viewValue;
                }
                return combined;
            }, function (value) {
                if (value) {
                    ctrl.$parsers.unshift(function (viewValue) {
                        var origin = scope.passwordVerify;
                        if (origin !== viewValue) {
                            ctrl.$setValidity("passwordVerify", false);
                            return undefined;
                        } else {
                            ctrl.$setValidity("passwordVerify", true);
                            return viewValue;
                        }
                    });
                }
            });
        }
    };
});