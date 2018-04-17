(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('mainCtrl', mainCtrl);

    mainCtrl.$inject = ['$scope'];

    function mainCtrl($scope) {
        $scope.masterTitle = 'Main Controller';
        $scope.profile = {};

        $scope.onViewProfile = function (prof,role,id) {
            $scope.profile = JSON.parse(prof);
            $scope.profile.Role = role;
            $scope.profile.AccountId = id;
            $('.modal-bg').css('background-image', 'url(' + $scope.profile.PhotoUrl + ')');

            fetchProfile(id);
        }

        function fetchProfile(account) {
            $scope.profileLoader = true;
            $.ajax({
                method: 'GET',
                url: '/api/accounts/' + account + '/profile',
                success: function (res) {
                    onFetchSuccess(res);
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                    $scope.$apply(function () {
                        $scope.profileLoader = false;
                    });
                }
            })
            
        }
        function onFetchSuccess(data) {
            console.log(data);
            $scope.$apply(function () {
                $scope.profile.Email = data.email;
                if (data.regNo) {
                    $scope.profile.RegNo = data.regNo;
                }
                $scope.profileLoader = false;
            })
        }
    }
})();
