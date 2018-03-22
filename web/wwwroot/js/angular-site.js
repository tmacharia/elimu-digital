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
                    $scope.questions.push($scope.question);

                    $scope.question = {
                        Answers: []
                    }
                }
            }
        }
        $scope.onSubmitExam = function () {
            $scope.exam = {};
            $scope.exam.Date = $scope.examDate;
            $scope.exam.StartTime = $scope.StartTime;
            $scope.exam.EndTime = $scope.EndTime;
            $scope.exam.Questions = $scope.questions;

            //console.log($scope.exam);
            $.ajax({
                method: 'POST',
                url: '/api/exams/create/' + $('#UnitId').val(),
                type: 'json',
                data: { model: $scope.exam },
                success: function (res) {
                    console.log(res);
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
    }
})();