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

    examCtrl.$inject = ['$scope','$timeout','$interval'];

    function examCtrl($scope, $timeout,$interval) {
        $scope.title = '';
        $scope.exams = [];
        $scope.questions = [];
        $scope.question = {
            Answers: []
        };

        $scope.calendarDate = function (date) {
            return moment().to(date);
        }
        $scope.minutesToExam = function (date) {
            var x = new moment(date);
            var now = new moment();
            var duration = moment.duration(x.diff(now)).as('minutes');
            duration = roundOff(duration, 2);
            return duration;
        }

        $scope.onInitExamIndex = function () {
            $scope.title = 'Examinations';
            //$scope.loader = true;
            fetchExams();
        }
        $scope.onInitUnitExams = function (id) {
            fetchExamsByUnit(id);
        }
        $scope.initExamSession = function (id) {
            $scope.title = 'Session';
            fetchExamSession(id);
        }
        $scope.initCourseWrkProgress = function (id) {
            fetchCourseWrkProgress(id);
        }
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
                warning('Specify exam title/name. e.g Supplementary exam, Final year exam e.t.c');
                return;
            }

            if ($scope.examDate === undefined) {
                warning('Pick a date for this exam.');
                return;
            }

            if ($scope.StartTime === undefined || $scope.EndTime === undefined) {
                warning('Pick start and end time for this exam.');
                return;
            }

            if ($scope.questions < 1) {
                warning('Exam has no questions. Add at-least one question to continue.');
                return;
            }

            postExam();
        }
        $scope.onSelectExam = function (exam) {
            $scope.selectedExam = exam;
            $('#examDetailsModal').modal('show');
            fetchExamDetails(exam.id);
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
        function fetchExams() {
            $scope.loader = true;
            $scope.exams = [];
            $.ajax({
                method: 'GET',
                url: '/api/exams',
                success: function (res) {
                    if (res instanceof Array) {
                        if (res.length < 1) {
                            info('You have no exams scheduled yet!');
                        }
                        else {
                            for (var i = 0; i < res.length; i++) {
                                $scope.$apply(function () {
                                    $scope.exams.push(res[i]);
                                })
                            }
                        }
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

            $scope.loader = false;
        }
        function fetchExamDetails(id) {
            $scope.detailsLoader = true;

            $.ajax({
                method: 'GET',
                url: '/api/exams/' + id,
                success: function (res) {
                    $scope.$apply(function () {
                        $scope.selectedExam = res;
                        
                    })
                    onDetailsSuccess();
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                }
            });

            $scope.detailsLoader = false;
        }
        function onDetailsSuccess() {
            $('.accordion-section-title').click(function (e) {
                // Grab current anchor value
                var currentAttrValue = $(this).attr('href');

                if ($(e.target).is('.active')) {
                    close_accordion_section();

                    // Add icon
                    e.target.children[1].firstElementChild.className = 'fa fa-plus';
                }
                else {
                    close_accordion_section();

                    // Add active class to section title
                    $(this).addClass('active');

                    // Add icon
                    e.target.children[1].firstElementChild.className = 'fa fa-minus';

                    // Open up the hidden content panel
                    $('.accordion ' + currentAttrValue).slideDown(300).addClass('open');
                }

                e.preventDefault();
            });
        }
        function close_accordion_section() {
            $('.accordion .accordion-section-title').removeClass('active');
            $('.accordion .accordion-section-content').slideUp(300).removeClass('open');
        }
        function fetchExamsByUnit(id) {
            $scope.loader = true;
            $scope.exams = [];
            $.ajax({
                method: 'GET',
                url: '/api/exams/unit/'+id,
                success: function (res) {
                    if (res instanceof Array) {
                        if (res.length < 1) {
                            info('You have no exams scheduled yet!');
                        }
                        else {
                            for (var i = 0; i < res.length; i++) {
                                $scope.$apply(function () {
                                    $scope.exams.push(res[i]);
                                })
                            }
                        }
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

            $scope.loader = false;
        }
        function fetchExamSession(id) {
            $scope.loader = true;

            $.ajax({
                method: 'GET',
                url: '/api/exams/' + id + '/session',
                success: function (res) {
                    onFetchSessionSuccess(res);
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                }
            });
            $scope.loader = false;
        }
        function fetchCourseWrkProgress(id) {
            $scope.courseWrkLoader = {};
            $scope.courseWrkLoader.IsActive = true;
            $scope.courseWrkLoader.Text = 'loading coursework progress...';

            $.ajax({
                method: 'GET',
                url: '/api/progress/unit/' + id + '/my',
                success: function (res) {
                    if (res.data.length > 0) {
                        $scope.$apply(function () {
                            $scope.courseworkprogress = res.data;
                            $scope.courseWrkLoader.IsActive = false;
                            $scope.courseWrkLoader.Text = '';
                        })
                    } else {
                        $scope.$apply(function () {
                            $scope.courseWrkLoader.IsActive = false;
                            info('This unit has not coursework materials uploaded.');
                        })
                    }
                },
                error: function (res) {
                    $scope.courseWrkLoader.IsActive = false;
                    $scope.courseWrkLoader.Text = '';

                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                }
            });
        }
        function onFetchSessionSuccess(data) {
            $scope.$apply(function () {
                $scope.title = 'Exam Session, Id: ' + data.sessionId.substring(0, 6);
                $scope.session = data;

                $('#rootwizard').bootstrapWizard({
                    onTabShow: function (tab, navigation, index) {
                        if (index > 0) {
                            var $total = navigation.find('li').length - 1;
                            var $current = index + 1;
                            var $percent = ($current / $total) * 100;
                            $('#rootwizard .progress-bar').css({ width: $percent + '%' });
                        }
                    }
                });
            });
            console.log(data);
        }
    }
})();

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