appointmentReminderApp.controller('authRegisterController',
	function AuthFormController($scope, $window, $location, $routeParams, authService) {
	    $scope.savedSuccessfully = false;
        $scope.message = "";
	    $scope.registerForm = function () {
	        authService.registerUser($scope.auth)
                .success(function (data, status, headers, config) {
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