
var appointmentReminderApp = angular.module('appointmentReminderApp', ["ngRoute", "ui.bootstrap", "LocalStorageModule"]);

appointmentReminderApp.config(function ($routeProvider, $locationProvider, $httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
    $locationProvider.html5Mode(true);
    $routeProvider
        // Auth 
		.when("/home", {
		    templateUrl: "App/Home.html",
		    controller: "HomeController"
		})
		.when("/Register", {
		    templateUrl: "App/AuthForm/templates/register.html",
		    controller: "authRegisterController"
		})
		.when("/RegisterSuccess", {
		    templateUrl: "App/AuthForm/templates/registerSuccess.html",
		    controller: "authRegisterController"
		})
		.when("/Login", {
		    templateUrl: "App/AuthForm/templates/login.html",
		    controller: "authLoginController"
		})
		.when("/Logout", {
		    templateUrl: "App/AuthForm/templates/logout.html",
		    controller: "authLogoutController"
		})
		.when("/ResetPassword", {
        templateUrl: "App/AuthForm/templates/resetPassword.html",
            controller: "authRegisterController"
		})
		.when("/PasswordResetCheck", {
        templateUrl: "App/AuthForm/templates/resetPasswordCheck.html",
            controller: "authRegisterController"
    })
        // Profile
		.when("/Profile", {
		    templateUrl: "App/ProfileForm/templates/profile.html",
		    controller: "profileController"
		})
		.when("/Profile/:id", {
		    templateUrl: "App/ProfileForm/templates/profile.html",
		    controller: "profileController"
		})
		.when("/newProfileForm", {
		    templateUrl: "App/ProfileForm/templates/profileUpdate.html",
		    controller: "profileController"
		})
		.when("/updateProfileForm/:id", {
		    templateUrl: "App/ProfileForm/templates/profileUpdate.html",
		    controller: "profileController"
		})
        // Contact
		.when("/Contacts", {
		    templateUrl: "App/ContactForm/templates/contacts.html",
		    controller: "contactController"
		})
		.when("/newContactForm", {
		    templateUrl: "App/ContactForm/templates/contactUpdate.html",
		    controller: "contactController"
		})
		.when("/updateContactForm/:id", {
		    templateUrl: "App/ContactForm/templates/contactUpdate.html",
		    controller: "contactController"
		})
		.when("/deleteContactForm/:id", {
		    templateUrl: "App/ContactForm/templates/contactDelete.html",
		    controller: "contactController"
		})
        // Reminder
		.when("/Reminders", {
		    templateUrl: "App/ReminderForm/templates/reminders.html",
		    controller: "reminderController"
		})
		.when("/historyReminderForm", {
		    templateUrl: "App/ReminderForm/templates/reminderHistory.html",
		    controller: "reminderController"
		})
		.when("/newReminderForm", {
		    templateUrl: "App/ReminderForm/templates/reminderUpdate.html",
		    controller: "reminderController"
		})
		.when("/updateReminderForm/:id", {
		    templateUrl: "App/ReminderForm/templates/reminderUpdate.html",
		    controller: "reminderController"
		})
		.when("/deleteReminderForm/:id", {
		    templateUrl: "App/ReminderForm/templates/reminderDelete.html",
		    controller: "reminderController"
		})
        // All else
		.otherwise({ redirectTo: "/home" });
});

