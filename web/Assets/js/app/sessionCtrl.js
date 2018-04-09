(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('sessionCtrl', sessionCtrl);

    sessionCtrl.$inject = ['$scope', '$interval'];
    /*!
     * Mock implementation of Threading
    */
    

    function sessionCtrl($scope,$interval) {
        $scope.title = 'Exam Session Controller';
        $scope.session = {};
        $scope.questions = [];
        $scope.selectedQue = {};
        $scope.targetTime = null;

        $scope.initSession = function (id) {
            fetchExamSession(id);
        }
        $scope.getIndex = function (que) {
            return $scope.questions.indexOf(que) + 5;
        }
        $scope.onNext = function () {
            
        }
        $scope.onPrev = function () {
            navigate(getCurrentIndex() - 1);
        }
        function activate() {
            $('footer ul li').on('click', function (e) {
                var index = parseInt(e.target.innerText) - 1;
                activateCurrent(index);
                $scope.$apply(function () {
                    $scope.selectedQue = $scope.questions[index];
                })
            });
            $('.next').on('click', function (e) {
                var i = getCurrentIndex() + 1;
                navigate(i);
            });
            $('.prev').on('click', function (e) {
                var i = getCurrentIndex() - 1;
                navigate(i);
            });
        }

        function navigate(val) {
            var total = $scope.questions.length;
            if (val < 0) {
                
            }
            else if (val > (total-1)) {
                
            }
            else {
                activateCurrent(val);
                $scope.$apply(function () {
                    $scope.selectedQue = $scope.questions[val];
                })
            }
        }
        function getCurrentIndex() {
            return $scope.questions.indexOf($scope.selectedQue);
        }
        function activateCurrent(current) {
            $('footer ul li').each(function (e) {
                if (e !== current) {
                    $(this).removeClass('active');
                } else {
                    $(this).addClass('active');
                }
            })
        }
        function fetchExamSession(id) {
            // loading active
            $('#loadingModal').modal('show');
            setTimeout(function () {
                $.ajax({
                    method: 'GET',
                    url: '/api/exams/sessions/' + id,
                    success: function (res) {
                        console.log(res);
                        onFetchSessionSuccess(res);
                    },
                    error: function (res) {
                        if (res.responseText) {
                            parseError(res.responseText);
                        } else {
                            error(res.statusText);
                        }
                    }
                })
                $('#loadingModal').modal('hide');
            }, 2000);
        }
        function onFetchSessionSuccess(data) {
            $scope.$apply(function () {
                // refresh session data
                $scope.session = data;
                // refresh timer
                $scope.targetTime = moment(data.exam.end);
                $scope.questions = data.exam.questions;
            });

            // refresh questions navigator
            $('footer ul').empty();
            for (var i = 0; i < $scope.questions.length; i++) {
                $('footer ul').append('<li>' + (i + 1) + '</li>');
            }

            // mark active-question
            if ($scope.questions.length > 0) {
                activateCurrent(0);
                $scope.$apply(function () {
                    $scope.selectedQue = $scope.questions[0];
                });
            }

            // start timer
            $interval(onTimerTick, 1000);

            // re-activate event handlers
            activate();
        }
        function onTimerTick() {
            // subtract 1 second from moment time
            var mt = moment($scope.targetTime).subtract(1, 'seconds');
            // get current time
            var now = new moment();
            var duration = moment.duration(mt.diff(now));
            var hrs, mins, secs;
            hrs = duration.get('hours');
            mins = duration.get('minutes');
            secs = duration.get('seconds');

            // display new time
            $('.remaining-time').text(hrs + ':' + mins + ':' + secs);
        }
    }
})();
