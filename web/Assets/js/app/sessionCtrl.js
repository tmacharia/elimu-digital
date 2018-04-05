(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('sessionCtrl', sessionCtrl);

    sessionCtrl.$inject = ['$scope','$interval'];

    function sessionCtrl($scope,$interval) {
        $scope.title = 'Exam Session Controller';
        $scope.questions = [];
        $scope.selectedQue = {};
        $scope.targetTime = null;

        activate();

        $scope.initQues = function () {
            activate();
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
            $scope.questions.push({ text: 'Who?' });
            $scope.questions.push({ text: 'What?' });
            $scope.questions.push({ text: 'Which?' });
            $scope.questions.push({ text: 'Why?' });
            $scope.questions.push({ text: 'When?' });

            for (var i = 0; i < $scope.questions.length; i++) {
                $('footer ul').append('<li>' + (i + 1) + '</li>');
            }

            activateCurrent(0);
            $scope.selectedQue = $scope.questions[0];
            $scope.targetTime = moment().add(150, 'seconds');

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
            //$interval(function () {
            //    // subtract 1 second from moment time
            //    var mt = moment($scope.targetTime).subtract(1, 'seconds');
            //    // get new time
            //    var now = new moment();
            //    var duration = moment.duration(mt.diff(now));
            //    console.log(duration);
            //    var hrs, mins, secs;
            //    hrs = duration.get('hours');
            //    mins = duration.get('minutes');
            //    secs = duration.get('seconds');

            //    // display new time
            //    $('.remaining-time').text(hrs + ':' + mins + ':' + secs);
            //}, 1000);
        }

        function navigate(val) {
            var total = $scope.questions.length;
            console.log(val);
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
    }
})();
