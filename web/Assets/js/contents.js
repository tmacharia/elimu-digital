var players;

(function () {
    // This is the bare minimum JavaScript. You can opt to pass no arguments to setup.
    // e.g. just plyr.setup(); and leave it at that if you have no need for events
    players = plyr.setup({
        // Output to console
        debug: false
    });
    players[0].on('ended', onExit);
    // Get an element
    function get(selector) {
        return document.querySelector(selector);
    }

    // Custom event handler (just for demo)
    function on(element, type, callback) {
        if (!(element instanceof HTMLElement)) {
            element = get(element);
        }
        element.addEventListener(type, callback, false);
    }

})();

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
    var id = parseInt($('#CommentId').text());
    
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
          console.log(res);
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
            console.log(res);
            if (res.isComplete) {
                yay('Coursework completed! Please refresh page.')
            }
        },
        error: function (res) {
            parseError(res.responseText);
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