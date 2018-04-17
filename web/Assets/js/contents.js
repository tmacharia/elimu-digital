function onLikeContent(id, title, elem) {
    $(elem).prop('disabled', true);
    postLike(id,elem);
}
function postLike(id,elem) {
    $.ajax({
        method: 'POST',
        url: '/api/contents/' + id + '/like',
        success: function (res) {
            yay(res);
            updateLikes();
        },
        error: function (res) {
            if (res.status === 403) {
                error('You are not authorized to perform this action');
            } else {
                parseError(res.responseText);
            }
            
        }
    });

    $(elem).prop('disabled', false);
}
function updateLikes() {
    var elem = $('.likes-count');

    var count = parseInt(elem.text());

    count += 1;

    elem.text(count);
}
function onComment() {
    var id = parseInt($('#ContentId').text());
    
    var comment = {};

    comment.Message = $('#CommentBox')[0].innerHTML;
    loadingBtn('postCommentBtn', true);
    
    $.ajax({
        method: 'POST',
        url: '/api/contents/'+id+'/comment',
        type: 'json',
        data: { model: comment },
        success: function (res) {
            yay(res);
            setTimeout(function () {
                window.location.reload();
            }, 1000);
        },
        error: function (res) {
            parseError(res.responseText);
        }
    })
}
function postInitialProgress(progressId, current, total)
{
    $.ajax({
        method: 'POST',
        url: '/api/progress/initialize',
        data: { id: progressId, current: current, overall: total},
        type: 'json',
        success: function (res) {
            yay('Progress updated!');
        },
        error: function (res) {
            parseError(res.responseText);
        }
    })
}
function postProgress(id, current) {
    $.ajax({
        method: 'POST',
        url: '/api/progress',
        data: { id: id, current: current},
        type: 'json',
        success: function (res) {
            if (res.isComplete) {
                yay('Coursework completed! Please refresh page.')
            }
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
function onDownloadContent(id) {
    console.log('Downloading...');
    $.ajax({
        method: 'POST',
        url: '/api/progress/download',
        data: { id: id },
        type: 'json'
    })
}
function onDeleteContent(id,unitId,unitName) {
    $.confirm({
        theme: 'supervan',
        title: 'Delete this record?',
        content: 'Are you sure you want to delete this course?',
        type: 'red',
        typeAnimated: true,
        buttons: {
            yes: {
                text: 'Delete',
                btnClass: 'btn-red',
                action: function () {
                    deleteContent(id);
                }
            },
            No: function () {

            }
        }
    })
}
function deleteContent(id,unitId,unitName) {
    $.confirm({
        theme: 'supervan',
        content: function () {
            var self = this;

            return $.ajax({
                url: '/api/contents/' + id,
                dataType: 'json',
                method: 'DELETE',
            }).fail(function (response) {
                if (response.status === 200) {
                    self.setType('green');
                    self.setIcon('fa fa-check-circle');
                    self.setTitle('Record deleted successfully.');
                    self.setContent(response.responseText);
                    self.onAction = function () {
                        setTimeout(function () {
                            window.history.go(-2);
                        }, 500);
                    }

                } else {
                    self.setType('red');
                    self.setIcon('fa fa-ban');
                    self.setContent(response.responseText);
                    self.setTitle(response.statusText);
                }
            })
        }
    })
}