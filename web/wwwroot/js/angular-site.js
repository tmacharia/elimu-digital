(function () {
    'use strict';

    angular.module('gobel-app', [
        // Angular modules 
        'ngMaterialDatePicker'
        // Custom modules 

        // 3rd Party Modules

    ]);
})();
(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('examCtrl', examCtrl);

    examCtrl.$inject = ['$scope'];

    function examCtrl($scope) {
        $scope.questions = [];
        $scope.question = {
            Answers: []
        };

        $scope.onAddQuestion = function () {
            $scope.question = {
                Answers: []
            };
            $('#addQuestionModal').modal('show');
        }
        $scope.onAddAnswer = function () {
            if (!$scope.question.Text) {
                error('Type a question first before adding answer options.');
            } else {
                $scope.question.Answers.push($scope.answer);
                $scope.answer = {};

            }
        }
        $scope.onExitQuestion = function () {
            $scope.question = {
                Answers: []
            };
            $('#addQuestionModal').modal('hide');
        }
        $scope.onRemoveQuestion = function (que) {
            var index = $scope.questions.indexOf(que);
            $scope.questions.splice(index);
        }
        $scope.onSubmitQuestion = function () {
            if (!$scope.question) {
                error('No question written.');
            } else {
                if ($scope.question.Answers.length < 1) {
                    error('Your question has no answers. Please add answer options');
                } else {
                    var _marks = parseFloat($scope.question.Marks);
                    $scope.question.Marks = _marks;
                    $scope.questions.push($scope.question);

                    $scope.question = {
                        Answers: []
                    }
                }
            }
        }
        $scope.onSubmitExam = function () {
            $scope.exam = {};
            $scope.exam.Name = $scope.Name;
            $scope.exam.Date = moment($scope.examDate).format();
            $scope.exam.Start = moment($scope.StartTime, 'hh:mm a').format();
            $scope.exam.End = moment($scope.EndTime, 'hh:mm a').format();
            $scope.exam.Questions = $scope.questions;

            if (!$scope.Name) {
                error('Specify exam title/name. e.g Supplementary exam, Final year exam e.t.c');
                return;
            }

            if ($scope.examDate === undefined) {
                error('Choose a date for this exam.');
                return;
            }

            if ($scope.StartTime === undefined || $scope.EndTime === undefined) {
                error('Choose start and end time for this exam.');
                return;
            }

            if ($scope.questions < 1) {
                error('Exam has no questions. Add at-least one question to continue.');
                return;
            }

            postExam();
        }

        function postExam() {
            $scope.loader = true;
            $.ajax({
                method: 'POST',
                url: '/api/exams/create',
                type: 'json',
                data: {
                    id: $('#UnitId').val(),
                    model: $scope.exam
                },
                success: function (res) {
                    $scope.loader = false;
                    yay('Exam scheduled successfully! Redirecting in a few...');

                    setTimeout(function () {
                        window.location = '/exams';
                    }, 1000);
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                    $scope.loader = false;
                }
            })
            $scope.loader = false;
        }
    }
})();