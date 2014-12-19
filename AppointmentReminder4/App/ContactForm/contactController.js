appointmentReminderApp.controller('contactController',
	function ContactFormController($scope, $window, $location, $routeParams,$filter, contactService, authService) {

	    $scope.contactsAvailable = false;
	    $scope.profileExist = false;
	    $scope.message = "";

	    $scope.timeZones = [
          { idZone: 'PST', descZone : 'Pacific Standard Time' },
          { idZone: 'MST', descZone : 'Mountain Stnadard Time' },
          { idZone: 'CST', descZone : 'Central Standard Time' },
          { idZone: 'EST', descZone : 'Eastern Standard Time' }
	    ];
		
		if ($routeParams.id) {
		
			contactService.getContact($routeParams.id).then(
				function (results) {
				    $scope.contactsAvailable = true;
				    $scope.contact = results.data;
				    $scope.contact.PhoneNumber = $filter("tel")($scope.contact.PhoneNumber);
				    var keepGoing = true;
				    var i = 0;
				    for (i = 0; i <= $scope.timeZones.length && keepGoing; i++) {
				        if ($scope.timeZones[i].idZone == $scope.contact.TimeZone) {
				            $scope.selectedTimeZone = $scope.timeZones[i];
				            keepGoing = false;
				        }
				    }

				},
				function (results) {
				    // on error
				    $scope.contactsAvailable = true;
					var data = results.data;
				}
			);

		} else {
			$scope.contact = { id: -1 };
			contactService.getContacts().then(
				function (results) {
				    $scope.contacts = results.data;

				    if ($scope.contacts == null)
				    {
				        $scope.message = "Please create profile first and then add contacts.";
				        $scope.contactsAvailable = false;
				        $scope.profileExist = false;
				    }
				    else
				    {
				        $scope.contactsAvailable = true;
				        $scope.profileExist = true;
				    }

					$scope.$broadcast('CONTACTS_LOADED_EVENT');
				},
				function (results) {
				    // on error
				    $scope.contactsAvailable = false;
				    $scope.profileExist = false;
					var data = results.data;
				}
			);
		}
		
		$scope.$on('CONTACTS_LOADED_EVENT', function () {
			$scope.loadingcontacts = false;
		});

		$scope.$on('CONTACTS_LOADING_EVENT', function () {
			$scope.loadingcontacts = true;
		});

		$scope.$broadcast('CONTACTS_LOADING_EVENT');
		
		$scope.showCreateContactForm = function () {
			$location.path('/newContactForm');
		};
		
		$scope.showUpdateContactForm = function (id) {
			$location.path('/updateContactForm/' + id);
		};
		
		$scope.showDeleteContactForm = function (id) {
			$location.path('/deleteContactForm/' + id);
		};

		$scope.submitForm = function() {
		    if ($scope.contact.Id > 0) {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
				contactService.updateContact($scope.contact).then(
					function(results) {
						$scope.contact = results.data;
						$window.history.back();
					},
					function(results) {
						// on error
						var data = results.data;
					}
				);
			} else {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
				contactService.insertContact($scope.contact).then(
					function (results) {
						$scope.contact = results.data;
						$window.history.back();
					},
						function (results) {
							// on error
							var data = results.data;
						}
					);
		    }
            authService.checkContact();
		};
		
		$scope.submitDeleteForm = function () {
		    if ($scope.contact.Id > 0) {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
				contactService.deleteContact($routeParams.id).then(
					function (results) {
					    $scope.contact = results.data;
                        authService.checkContact();
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

