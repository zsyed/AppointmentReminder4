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

	    $scope.contactsalreadyLoaded = false;
	    $scope.contactsLoaded = false;

	    $scope.$watch('contactsalreadyLoaded', function (value) {
	        $scope.contactsLoaded = value;
	    });

	    var onContactsGetComplete = function (data) {
	        $scope.contacts = data;
	        if ($scope.contacts != null) {
	            $scope.contactsAvailable = true;
	            $scope.profileExist = true;
	        }
	        else
	        {
	            $scope.message = "Please create profile first and then add contacts.";
	            $scope.contactsAvailable = false;
	            $scope.profileExist = false;
	        }

	        $scope.contactsalreadyLoaded = true;
	    };

	    var onErrorContacts = function (reason) {
	        $scope.contactsalreadyLoaded = true;
	        $scope.error = "Could not fetch contacts data."
	        $scope.contactsAvailable = false;
	        $scope.profileExist = false;
	    };

	    var onContactGetComplete = function (data) {
		    $scope.contact = data;
		    $scope.contact.PhoneNumber = $filter("tel")($scope.contact.PhoneNumber);
		    var keepGoing = true;
		    var i = 0;
		    for (i = 0; i <= $scope.timeZones.length && keepGoing; i++) {
			    if ($scope.timeZones[i].idZone == $scope.contact.TimeZone) {
				    $scope.selectedTimeZone = $scope.timeZones[i];
				    keepGoing = false;
				    }
		    }
	    };

	    var onErrorContact = function (reason) {
	        $scope.error = "Could not load contact";
	    };

		if ($routeParams.id) {
            contactService.getContact($routeParams.id).then(onContactGetComplete, onErrorContact);
		} else {
		    $scope.contact = { id: -1 };
		    contactService.getContacts().then(onContactsGetComplete, onErrorContacts);
		}
		
		$scope.showCreateContactForm = function () {
			$location.path('/newContactForm');
		};
		
		$scope.showUpdateContactForm = function (id) {
			$location.path('/updateContactForm/' + id);
		};
		
		$scope.showDeleteContactForm = function (id) {
			$location.path('/deleteContactForm/' + id);
		};

		var onContactInsertUpdateComplete = function (data) {
		    $scope.contact = data;
		    $window.history.back();
		};

		var onErrorUpdateContact = function (reason) {
		    $scope.error = "Could not update contact";
		};

		var onErrorInsertContact = function (reason) {
		    $scope.error = "Could not Insert contact";
		};


		$scope.submitForm = function() {
		    if ($scope.contact.Id > 0) {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
		        contactService.updateContact($scope.contact).then(onContactInsertUpdateComplete, onErrorUpdateContact);
			} else {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
		        contactService.insertContact($scope.contact).then(onContactInsertUpdateComplete, onErrorInsertContact);
		    }
            authService.checkContact();
		};
		
        var onContactDeleteComplete = function (data) {
            $scope.contact = data;
            authService.checkContact();
            $window.history.back();
    	};

	    var onErrorDeleteContact = function (reason) {
	        $scope.error = "Could not delete contact";
	    };

		$scope.submitDeleteForm = function () {
		    if ($scope.contact.Id > 0) {
		        $scope.contact.TimeZone = $scope.selectedTimeZone.idZone;
                contactService.deleteContact($routeParams.id).then(onContactDeleteComplete, onErrorDeleteContact);
			} 
		};
	});
