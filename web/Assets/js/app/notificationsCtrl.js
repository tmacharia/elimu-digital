(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('notificationsCtrl', notificationsCtrl);

    notificationsCtrl.$inject = ['$scope'];

    function notificationsCtrl($scope) {
        $scope.title = 'My Notifications';
        $scope.notifications = [];
        activate();

        $scope.initNotifications = function () {
            fetchMyNotifications();
        }
        $scope.normalizeDate = function (ntf) {
            return moment().to(ntf.timestamp);
        }
        $scope.onDelete = function (ntf) {
            deleteNotification(ntf);
        }

        function activate() { }
        function fetchMyNotifications() {
            $scope.loader = true;

            $.ajax({
                method: 'GET',
                url: '/api/notifications',
                success: function (res) {
                    onFetchSuccess(res);
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                    $scope.loader = false;
                }
            });
        }
        function onFetchSuccess(data) {
            if (data instanceof Array) {
                $scope.$apply(function () {
                    $scope.notifications = data;
                    $scope.loader = false;
                });
            } else {
                console.log(data);
            }
            
            setTimeout(function () {
                markAsRead();
            }, 10000);
        }
        function markAsRead() {
            var unread = $scope.notifications.filter(x => x.read === false);

            if (unread.length > 0) {
                $.ajax({
                    method: 'POST',
                    url: '/api/notifications/markasread',
                    type: 'json',
                    data: { notifications: unread },
                    success: function (res) {
                        if (res instanceof Array) {
                            $scope.$apply(function () {
                                $scope.notifications = res;
                            });
                        }
                    },
                    error: function (res) {
                        if (res.responseText) {
                            parseError(res.responseText);
                        } else {
                            error(res.statusText);
                        }
                    }
                });
            }
        }
        function deleteNotification(ntf) {
            $.ajax({
                method: 'DELETE',
                url: '/api/notifications/' + ntf.id,
                success: function (res) {
                    yay(res);
                    onDeleteSuccess(ntf);
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                }
            })
        }
        function onDeleteSuccess(ntf) {
            var index = $scope.notifications.indexOf(ntf);

            $scope.$apply(function () {
                $scope.notifications.splice(index, 1);
            })
        }
    }
})();
