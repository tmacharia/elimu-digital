(function () {
    'use strict';

    angular.module('gobel-app', [
        // Angular modules 
        'ngMaterialDatePicker', 'ngSanitize'
        // Custom modules 

        // 3rd Party Modules

    ]);
})();
(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('boardsCtrl', boardsCtrl);

    boardsCtrl.$inject = ['$scope'];

    function boardsCtrl($scope) {
        $scope.title = 'boardsCtrl';
        $scope.participants = [];
        $scope.posts = [];

        activate();

        $scope.initBoard = function (id) {
            fetchTopPosts(parseInt(id));
            fetchParticipants(parseInt(id));
        }
        $scope.onNewPost = function (id) {
            postOnBoard(id, $('#CommentBox').text());
            $('#CommentBox').text('');
        }
        $scope.onLike = function (post) {
            pushLikePost(post);
        }
        $scope.onNewComment = function (post) {
            pushCommentOnPost(post, post.comment);
            post.comment = '';
        }

        function activate() { }

        function fetchTopPosts(id) {
            $scope.postsLoader = true;

            $.ajax({
                method: 'GET',
                url: '/api/discussionboards/' + id + '/posts',
                success: function (res) {
                    if (res instanceof Array) {
                        $scope.$apply(function () {
                            $scope.posts = res;
                        })
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

            $scope.postsLoader = false;
        }
        function fetchParticipants(id) {
            $scope.participantsLoader = true;

            $.ajax({
                method: 'GET',
                url: '/api/discussionboards/' + id + '/participants',
                success: function (res) {
                    if (res instanceof Array) {
                        $scope.$apply(function () {
                            $scope.participants = res;
                        })
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

            $scope.participantsLoader = false;
        }
        function postOnBoard(id, msg) {
            loadingBtn('postSubmitBtn', true);

            $.ajax({
                method: 'POST',
                url: '/api/discussionboards/' + id + '/post?message=' + msg,
                success: function (res) {
                    yay('Posted!');
                    $scope.$apply(function () {
                        $scope.posts.insert(0, res);
                    })
                },
                error: function (res) {
                    if (res.responseText) {
                        parseError(res.responseText);
                    } else {
                        error(res.statusText);
                    }
                }
            });

            loadingBtn('postSubmitBtn', false);
        }
        function pushLikePost(post) {
            $.ajax({
                method: 'POST',
                url: '/api/posts/' + post.id + '/like',
                success: function (res) {
                    yay('Liked!');
                    $scope.$apply(function () {
                        post.likes = res.likes;
                    })
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
        function pushCommentOnPost(post,msg) {
            $.ajax({
                method: 'POST',
                url: '/api/posts/' + post.id + '/comment',
                type: 'json',
                data: { model: {Message:msg} },
                success: function (res) {
                    yay('Commented!');
                    $scope.$apply(function () {
                        if (post.comments === null) {
                            post.comments = [];
                        }

                        post.comments.insert(0, res);
                    })
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

(function () {
    'use strict';

    angular
        .module('gobel-app')
        .controller('examCtrl', examCtrl);

    examCtrl.$inject = ['$scope','$interval'];

    function examCtrl($scope, $interval) {
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
                            $scope.$apply(function () {
                                $scope.exams = res;
                            })
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
                url: '/api/exams/sessions/' + id,
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
        }
    }
})();

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