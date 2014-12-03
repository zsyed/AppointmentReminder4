
var appointmentReminderApp = angular.module('appointmentReminderApp', ["ngRoute", "ui.bootstrap", "LocalStorageModule"]);

appointmentReminderApp.config(function ($routeProvider, $locationProvider) {
// appointmentReminderApp.config(function ($routeProvider, $locationProvider,$httpProvider) {
   // $httpProvider.interceptors.push('authInterceptorService');
    $locationProvider.html5Mode(true);

    $routeProvider
		.when("/home", {
		    templateUrl: "App/Home.html",
		    controller: "HomeController"
		})
		.when("/Register", {
		    templateUrl: "App/AuthForm/templates/register.html",
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
		.when("/values", {
		    templateUrl: "App/AuthForm/templates/values.html",
		    controller: "ValuesController"
		})
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
		.otherwise({ redirectTo: "/home" });
});

appointmentReminderApp.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});