appointmentReminderApp.controller('reminderController',
	function ReminderFormController($scope, $filter, $window, $location, $routeParams, reminderService) {

	    $scope.doneloadingreminders = false;
	    $scope.finishedloadingreminders = false;
	    $scope.loadedreminderhistories = false;
	    $scope.finishedreminderhistories = false;

	    var onReminderHistoriesGetComplete = function (data) {
	        $scope.reminderhistories = data;
	        $scope.loadedreminderhistories = true;
	    };

	    var onErrorReminderHistories = function (reason) {
	        $scope.error = "Could not load reminder histories";
	    };

	    var onReminderGetComplete = function (data) {
	        $scope.reminder = data;
	        var contactId = $scope.reminder.ContactId;
	        var recurrence = $scope.reminder.Recurrence;
	        var weekday = $scope.reminder.WeekDay;

	        $scope.ReminderDate = $filter('date')($scope.reminder.ReminderDateTime, 'MM/dd/yyyy');
	        $scope.ReminderTime = $filter('date')($scope.reminder.ReminderDateTime, 'MM/dd/yyyy hh:mm a');
	        var keepGoing = true;
	        for (i = 0; i <= $scope.remindercontacts.length && keepGoing; i++) {
	            if ($scope.remindercontacts[i].Id == contactId) {
	                $scope.selectedContact = $scope.remindercontacts[i];
	                keepGoing = false;
	            }
	        }


	        keepGoing = true;
	        for (i = 0; i <= $scope.recurrences.length && keepGoing; i++) {
	            //if ($scope.recurrences[i].idrecur == recurrence) {
	            if ($scope.recurrences[i].id == recurrence) {
	                $scope.selectedRecurrence = $scope.recurrences[i];
	                keepGoing = false;
	            }
	        }

	        if (weekday != null) {
	            keepGoing = true;
	            for (var i = 0; i <= $scope.weekdays.length && keepGoing; i++) {
	                if ($scope.weekdays[i].idweek == weekday) {
	                    $scope.selectedWeekday = $scope.weekdays[i];
	                    keepGoing = false;
	                }
	            }
	        }

	        //copy paste code must be refactored.

	        if (recurrence == 0) { //'Once'
	            $scope.calendarshow = true;
	            $scope.weekdayshow = false;
	        }

	        if (recurrence == 1) { //'Daily'
	            $scope.calendarshow = false;
	            $scope.weekdayshow = false;
	        }

	        if (recurrence == 2) { //
	            $scope.calendarshow = false;
	            $scope.weekdayshow = true;
	        }

	        //copy paste code
	    };

	    var onErrorReminder = function (reason) {
	        $scope.error = "Could not load reminder";
	    };

	    $scope.$watch('finishedloadingreminders', function (value) {
	        $scope.doneloadingreminders = value;
	    });

	    $scope.$watch('loadedreminderhistories', function (value) {
	        $scope.finishedreminderhistories = value;
	    });


        var onReminderContactsGetComplete = function (data) {
            $scope.remindercontacts = data;
        };

        var onErrorContactsReminder = function (reason) {
            $scope.error = "Could not load reminder contact. Or Contact may not exist. Please double check to see that at least 1 contact exists.";
        };

        reminderService.getReminderContacts().then(onReminderContactsGetComplete, onErrorContactsReminder);

        var onRemindersGetComplete = function (data) {
            $scope.reminders = data;
            $scope.finishedloadingreminders = true;
        };

        var onErrorReminders = function (reason) {
            $scope.error = "Could not load reminders";
        };

		if ($routeParams.id) {
		    reminderService.getReminder($routeParams.id).then(onReminderGetComplete, onErrorReminder);
		} else {
		    $scope.reminder = { id: -1 };
		    reminderService.getReminders().then(onRemindersGetComplete, onErrorReminders);
            reminderService.getReminderHistories().then(onReminderHistoriesGetComplete, onErrorReminderHistories);
		}

		//$scope.recurrences = [
		//  { idrecur: 'Once' },
		//  { idrecur: 'Daily' },
		//  { idrecur: 'Weekly' }
	    //];

		$scope.recurrences = [
		  { id: 0, option: 'Once' },
		  { id: 1, option: 'Daily' },
		  { id: 2, option: 'Weekly' }
		];

		$scope.weekdays = [
			{ idweek: 'Monday' },
			{ idweek: 'Tuesday' },
			{ idweek: 'Wednesday' },
			{ idweek: 'Thursday' },
			{ idweek: 'Friday' },
			{ idweek: 'Saturday' },
			{ idweek: 'Sunday' }
		];
		
		$scope.showCreateReminderForm = function () {
			$location.path('/newReminderForm');
		};

		$scope.showHistoryReminderForm = function () {
			$location.path('/historyReminderForm');
		};

		$scope.showUpdateReminderForm = function (id) {
			$location.path('/updateReminderForm/' + id);
		};

		$scope.showDeleteReminderForm = function (id) {
			$location.path('/deleteReminderForm/' + id);
		};

		$scope.submitForm = function () {
			if ($scope.reminder.Id > 0) {
				
				$scope.reminder.ContactId = $scope.selectedContact.Id;

				if ($scope.calendarshow) {
					$scope.ReminderDate = $filter('date')($scope.ReminderDate, 'MM/dd/yyyy');
				} else {
					$scope.ReminderDate = '01/01/1901';
				}

				$scope.ReminderTime = $filter('date')($scope.ReminderTime, 'hh:mm a');
				$scope.ReminderTime = $scope.ReminderTime.substr($scope.ReminderTime.length - 8);
				$scope.reminder.ReminderDateTime = $scope.ReminderDate + " " + $scope.ReminderTime;
				//$scope.reminder.Recurrence = $scope.selectedRecurrence.idrecur;
				$scope.reminder.Recurrence = $scope.selectedRecurrence.id;

				if ($scope.weekdayshow) {
					$scope.reminder.WeekDay = $scope.selectedWeekday.idweek;
				}


				var onReminderUpdateComplete = function (data) {
				    $scope.reminder = data;
				    $window.history.back();
				};

				var onErrorUpdateReminder = function (reason) {
				    $scope.error = "Could not update reminder.";
				};

				reminderService.updateReminder($scope.reminder).then(onReminderUpdateComplete, onErrorUpdateReminder);

			} else {
				$scope.reminder.ContactId = $scope.selectedContact.Id;
				
				if ($scope.calendarshow) {
					$scope.ReminderDate = $filter('date')($scope.ReminderDate, 'MM/dd/yyyy');
				} else {
					$scope.ReminderDate = '01/01/1901';
				}

				$scope.ReminderTime = $filter('date')($scope.ReminderTime, 'hh:mm a');
				$scope.ReminderTime = $scope.ReminderTime.substr($scope.ReminderTime.length - 8);
				$scope.reminder.ReminderDateTime = $scope.ReminderDate + " " + $scope.ReminderTime;
				//$scope.reminder.Recurrence = $scope.selectedRecurrence.idrecur;
				$scope.reminder.Recurrence = $scope.selectedRecurrence.id;
				
				if ($scope.weekdayshow) {
					$scope.reminder.WeekDay = $scope.selectedWeekday.idweek;
				}

				var onReminderInsertComplete = function (data) {
				    $scope.reminder = data;
				    $window.history.back();
				};

				var onErrorInsertReminder = function (reason) {
				    $scope.error = "Could not insert reminder.";
				};
				
				reminderService.insertReminder($scope.reminder).then(onReminderInsertComplete, onErrorInsertReminder); 
			}
		};

		var onReminderDeleteComplete = function (data) {
		    $scope.reminder = data;
		    $window.history.back();
		};

		var onErrorDeleteReminder = function (reason) {
		    $scope.error = "Could not delete contact";
		};

		$scope.submitDeleteForm = function () {
		    if ($scope.reminder.Id > 0) {
		        reminderService.deleteReminder($routeParams.id).then(onReminderDeleteComplete, onErrorDeleteReminder);

			}
		};
		
		/* Time */
		$scope.hstep = 1;
		$scope.mstep = 15;
		$scope.ismeridian = true;
		
		$scope.options = {
			hstep: [1, 2, 3],
			mstep: [1, 5, 10, 15, 25, 30]
		};

		$scope.toggleMode = function () {
			$scope.ismeridian = !$scope.ismeridian;
		};
	    /* Time */

	    /* Calendar */

		$scope.today = function () {
		    $scope.dt = new Date();
		};
		$scope.today();

		$scope.clear = function () {
		    $scope.dt = null;
		};

		$scope.toggleMin = function () {
		    $scope.minDate = $scope.minDate ? null : new Date();
		};
		$scope.toggleMin();

		$scope.open = function ($event) {
		    $event.preventDefault();
		    $event.stopPropagation();

		    $scope.opened = true;
		};

		$scope.dateOptions = {
		    formatYear: 'yy',
		    startingDay: 1
		};

		$scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate', 'fullDate'];
		$scope.format = $scope.formats[4];

        /* Calendar */
		
		/* manage controls */
		$scope.calendarshow = false;
		$scope.weekdayshow = false;
		
		$scope.reminderRecurrenceTimeChanged = function () {
		    //var recurrence = $scope.selectedRecurrence.idrecur; // $('#ddlRecurrence option:selected').text();
		    var recurrence = $scope.selectedRecurrence.id; // $('#ddlRecurrence option:selected').text();
			if (recurrence == 0) { // 'Once'
				$scope.calendarshow = true;
				$scope.weekdayshow = false;
			}

			if (recurrence == 1) { //'Daily'
				$scope.calendarshow = false;
				$scope.weekdayshow = false;
			}

			if (recurrence == 2) { //'Weekly'
				$scope.calendarshow = false;
				$scope.weekdayshow = true;
			}
		};

		/* manage controls */
	});

