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
