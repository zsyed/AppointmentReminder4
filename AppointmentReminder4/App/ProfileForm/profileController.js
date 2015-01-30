appointmentReminderApp.controller('profileController',
	function ProfileFormController($scope, $window, $location, $routeParams,$filter, profileService, authService) {

	    $scope.profileLoaded = false;
	    $scope.profileShow = false;

	    $scope.$watch('profileLoaded', function (value) {
	        $scope.profileShow = value;
	    });

	    var onProfileGetComplete = function (data) {
	        $scope.profile = data;
	        if ($scope.profile != null)
	        {
	            $scope.profile.PhoneNumber = $filter("tel")($scope.profile.PhoneNumber);
	        }
        
	        $scope.profileLoaded = true;
	    };

	    var onError = function (reason)
	    {
	        $scope.profileLoaded = true;
            $scope.error = "Could not fetch profile data."
	    };

	    profileService.getProfile().then(onProfileGetComplete, onError);

		$scope.showCreateProfileForm = function () {
			$location.path('/newProfileForm');

		};

		$scope.showUpdateProfileForm = function (id) {
			$location.path('/updateProfileForm/' + id);

		};

		$scope.submitForm = function () {
		    if ($scope.profile.Id > 0) {
		        $scope.profile.PhoneNumber = $scope.profile.PhoneNumber.replace(/\D/g, '');
				profileService.updateProfile($scope.profile).then(
					function (results) {
					    $scope.profile = results.data;
					    if ($scope.profile != null)
					    {
					        authService.profileExists(true);
					    }

						$window.history.back();
						},
						function (results) {
						// on error
						var data = results.data;
						}
					);
			} else {
				
				profileService.insertProfile($scope.profile).then(
					function (results) {
					    $scope.profile = results.data;
					    if ($scope.profile != null) {
					        authService.profileExists(true);
					    }
						$window.history.back();
					},
						function (results) {
							// on error
							var data = results.data;
						}
					);
			}
		};

	});