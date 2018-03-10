(function () {
    // This is the bare minimum JavaScript. You can opt to pass no arguments to setup.
    // e.g. just plyr.setup(); and leave it at that if you have no need for events
    var instances = plyr.setup({
        // Output to console
        debug: false
    });

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

    // Loop through each instance
    instances.forEach(function (instance) {
        // Play
        on('.js-play', 'click', function () {
            instance.play();
        });

        // Pause
        on('.js-pause', 'click', function () {
            instance.pause();
        });

        // Stop
        on('.js-stop', 'click', function () {
            instance.stop();
        });

        // Rewind
        on('.js-rewind', 'click', function () {
            instance.rewind();
        });

        // Forward
        on('.js-forward', 'click', function () {
            instance.forward();
        });
    });
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
            parseError(res.responseText);
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
