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
	        authService.resetPassword($scope.passwordResetData).then(
                function (results) {
                    // $scope.auth = results.data;
                    // $window.history.back();
                    $location.path('/PasswordResetCheck');
                },
                    function (results) {
                        // on error
                        var data = results.data;
                    }
                );
	    };
	});