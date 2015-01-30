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

		var onProfileUpdateComplete = function (data) {
		    $scope.profile = data;
		    if ($scope.profile != null)
		    {
		        authService.profileExists(true);
		        $window.history.back();
		    }
		};

		var onUpdateError = function (reason) {
		    $scope.error = "Could not update profile data."
		};

		var onProfileInsertComplete = function (data) {
		    $scope.profile = data;
		    if ($scope.profile != null) {
		        authService.profileExists(true);
		    }
		    $window.history.back();
		};

		var onInsertError = function (reason) {
		    $scope.error = "Could not insert profile data."
		};

		$scope.submitForm = function () {
		    if ($scope.profile.Id > 0) {
		        $scope.profile.PhoneNumber = $scope.profile.PhoneNumber.replace(/\D/g, '');
		        profileService.updateProfile($scope.profile).then(onProfileUpdateComplete, onUpdateError);
			} else {
		        profileService.insertProfile($scope.profile).then(onProfileInsertComplete, onInsertError);
			}
		};

	});