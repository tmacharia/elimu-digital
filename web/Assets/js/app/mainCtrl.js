(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('mainCtrl', mainCtrl);

    mainCtrl.$inject = ['$scope'];

    function mainCtrl($scope) {
        $scope.masterTitle = 'Main Controller';
        $scope.profile = {};

        $scope.onViewProfile = function (account,role) {
            var acc = JSON.parse(account);
            $scope.profile = acc.Profile;
            $scope.profile.Role = role;
            console.log(acc);
        }
    }
})();
