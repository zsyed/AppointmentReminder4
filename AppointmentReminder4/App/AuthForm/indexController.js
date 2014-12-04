appointmentReminderApp.controller('indexController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.logoutUser = function () {
        authService.logoutUser();
        $location.path('/home');
    }

    $scope.authentication = authService.authentication;

}]);