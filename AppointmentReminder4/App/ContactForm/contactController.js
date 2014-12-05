appointmentReminderApp.controller('contactController',
	function ContactFormController($scope, $window, $location, $routeParams, contactService) {

	    $scope.timeZones = [
          { idZone: 'PST' },
          { idZone: 'MST' },
          { idZone: 'CST' },
          { idZone: 'EST'}
	    ];
		
		if ($routeParams.id) {
		
			contactService.getContact($routeParams.id).then(
				function (results) {

				    $scope.contact = results.data;

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
					var data = results.data;
				}
			);

		} else {
			$scope.contact = { id: -1 };
			contactService.getContacts().then(
				function (results) {
					$scope.contacts = results.data;
					$scope.$broadcast('CONTACTS_LOADED_EVENT');
				},
				function (results) {
					// on error
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
		};
		
		$scope.submitDeleteForm = function () {
		    if ($scope.contact.Id > 0) {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
				contactService.deleteContact($routeParams.id).then(
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
		};

	});

