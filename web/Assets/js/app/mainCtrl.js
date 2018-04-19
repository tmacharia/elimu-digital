(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('mainCtrl', mainCtrl);

    mainCtrl.$inject = ['$scope'];

    function mainCtrl($scope) {
        $scope.masterTitle = 'Main Controller';
        $scope.profile = {};
        $scope.lec = {
            Skills:[]
        }

        $scope.onViewProfile = function (prof,role,id) {
            $scope.profile = JSON.parse(prof);
            $scope.profile.Role = role;
            $scope.profile.AccountId = id;

            fetchProfile(id);
        }
        $scope.initLecBio = function (bio, json) {
            if (bio) {
                $scope.lec.Bio = bio;
            }
            if (json) {
                $scope.lec.Skills = JSON.parse(json);
            }
        }
        $scope.onNewSkill = function () {
            console.log('creating...');
            if (!$scope.lec.Skills) {
                $scope.lec.Skills = [];
            }
            $scope.lec.Skills.push($scope.skill);
            $scope.skill = {};
        }
        $scope.onRemoveSkill = function (skl) {
            var index = $scope.lec.Skills.indexOf(skl);
            $scope.lec.Skills.splice(index, 1);
        }
        $scope.onUpdate = function () {
            updateBio();
        }
        $scope.isSocialValid = function () {
            var count = 0;
            if ($scope.profile.Facebook) {
                count++;
            }
            if ($scope.profile.Twitter) {
                count++;
            }
            if ($scope.profile.Instagram) {
                count++;
            }

            if (count > 0) {
                return true;
            } else {
                return false;
            }
        }

        function updateBio() {
            loadingBtn('saveBioBtn', true);

            $.ajax({
                method: 'POST',
                url: '/api/lecturers/update',
                type: 'json',
                data: { model: $scope.lec },
                success: function (res) {
                    yay('Bio updated!');
                    
                    setTimeout(function () {
                        window.location.reload();
                    }, 1000);
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                }
            })

            loadingBtn('saveBioBtn', false);
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
