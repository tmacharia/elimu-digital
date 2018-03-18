var format = "hh:mm tt";
var classRoom = {}

$('#newClassForm').submit(function (e) {
    e.preventDefault();

    classRoom.DayOfWeek = $('#DayOfWeek').val();
    classRoom.StartTime = Date.parseExact($('#StartTime').val(), format).toLocaleString();
    classRoom.EndTime = Date.parseExact($('#EndTime').val(), format).toLocaleString();
    classRoom.Room = $('#Room').val();

    console.log(classRoom);
    postClass(classRoom);
})

function postClass() {
    loadingBtn('classSubmitBtn', true);
    $.ajax({
        method: 'POST',
        url: '/classes/create',
        type: 'json',
        data: { model: classRoom },
        success: function (response) {
            yay(response);
            loadingBtn('classSubmitBtn', false);

            setTimeout(function () {
                window.location = '/classes';
            }, 2000);
        },
        error: function (response) {
            loadingBtn('classSubmitBtn', false);
            if (response.responseText) {
                error(response.responseText);
            } else {
                error(response.statusText);
            }
        }
    })
}

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
var course = {};

$('#courseForm').submit(function (e) {
    e.preventDefault();

    var frm = $('#courseForm').serialize();

    console.log(frm);
    //postCourse();
});


function postCourse() {
    course.Name = $('#courseName').val();
    course.Type = $('#courseType').val();

    loadingBtn('btnSubmit', true);

    $.ajax({
        type: 'POST',
        url: '/api/courses',
        data: { schoolId: 1, model: course },
        dataType: 'json',
        error: function (response) {

            if (response.status === 200) {
                loadingBtn('btnSubmit', false);
                yay(response.responseText);
                setTimeout(function () {
                    location.reload();
                }, 700);
            } else {
                loadingBtn('btnSubmit', false);
                error(response.responseText);
            }
        }
    });
}

function onDeleteCourse(id) {
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
                    deleteCourse(id);
                }
            },
            No: function () {
                
            }
        }
    })
};

function deleteCourse(id) {
    $.confirm({
        theme: 'supervan',
        content: function () {
            var self = this;

            return $.ajax({
                url: '/api/courses/'+id,
                dataType: 'json',
                method: 'DELETE',
            }).fail(function (response) {
                if (response.status === 200) {
                    self.setType('green');
                    self.setIcon('fa fa-check-circle');
                    self.setTitle(response.responseText);
                    self.setContent('Record deleted successfully.');
                    self.onAction = function () {
                        setTimeout(function () {
                            location.reload();
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

function onSearch() {

}
var btn_text = '';

function loadingBtn(id, bool) {
    $('#' + id).prop('disabled', bool);

    if (bool) {
        btn_text = $('#' + id).text();

        $('#' + id).html("<i class='fa fa-circle-o-notch fa-spin'></i> posting...");
    } else {
        $('#' + id).text(btn_text);
    }
}

function yay(msg) {
    toastr.success(msg);
}

function error(msg) {
    toastr.error(msg);
}

function deleteConfirm() {
    
}

function disableLink(e) {
    e.preventDefault();

    $(this).prop('disabled', true);
}

function parseError(responseText) {
    var obj = JSON.parse(responseText);
    
    if (obj instanceof String) {
        error(responseText);
    } else {
        var msg = obj[Object.keys(obj)[0]];

        error(msg);
    }
}
var unit = {};
(function () {
    $('#unitBtnSubmit').click(function (e) {
        //e.preventDefault();

        if (_proc === 'Assign Lec') {
            //pushAssignLec();
        }
        else {
            postUnit();
        }

    });
})

function postUnit() {
    unit = {};

    unit.Name = $('#unitName').val();
    unit.Level = $('#Level').val();
    unit.Semester = $('#Semester').val();

    loadingBtn('unitBtnSubmit', true);

    $.ajax({
        type: 'POST',
        url: '/api/units',
        data: {
            courseId: $('#CourseId').val(),
            model: unit
        },
        dataType: 'json',
        error: function (response) {
            console.log(response);
            if (response.status === 200 || response.status === 201) {
                loadingBtn('unitBtnSubmit', false);
                yay(response.responseText);
                setTimeout(function () {
                    location.reload();
                }, 700);
            } else {
                loadingBtn('unitBtnSubmit', false);

                if (response.responseText) {
                    error(response.responseText);
                } else {
                    error(response.statusText);
                }
                
            }
        }
    });
}

function onAssignLecturer(name, id) {
    unit = {};
    unit.id = id;
    $('#unit_assign').text(name);
    $('#lecSelect').empty();

    $.ajax({
        method: 'GET',
        url: '/api/lecturers',
        success: function(response){
            $('#lecs-loader').hide();

            var o = new Option('-- Select --', '');
            $('#lecSelect').append(o);

            if (response.length > 0) {
                for (i = 0; i < response.length; i++) {
                    AddLecToOptions(response[i]);
                }
            }
        },
        error: function (response) {
            console.log(response);
        }
    })
}

function AddLecToOptions(lec) {
    var o = new Option(lec.profile.fullNames, lec.id);
    $('#lecSelect').append(o);
}

function pushAssignLec() {
    var lec_id = $('#lecSelect').val();

    $.ajax({
        method: 'POST',
        url: '/api/units/' + unit.id + '/assignLecturer/' + lec_id,
        success: function (response) {
            yay(response);

            setTimeout(function () {
                location.reload();
            }, 1000);
        },
        error: function (response) {
            error(response.responseText);
        }
    })
}

function onAllocateUnit(name, id) {
    unit = {};
    unit.id = id;
    $('#unit_allocate').text(name);
    $('#roomSelect').empty();

    $.ajax({
        method: 'GET',
        url: '/api/classes',
        success: function (response) {
            $('#rooms-loader').hide();

            var o = new Option('-- Select --', '');
            $('#roomSelect').append(o);

            if (response.length > 0) {
                for (i = 0; i < response.length; i++) {
                    AddRoomToOptions(response[i]);
                }
            }
        },
        error: function (response) {
            console.log(response);
        }
    })
}

function AddRoomToOptions(room) {
    var o = new Option(room.room + ', '+
        room.startTime+' - '+room.endTime+', '+
        room.dayOfWeek,room.id);
    $('#roomSelect').append(o);
}

function pushAllocateUnit() {
    var room_id = $('#roomSelect').val();

    $.ajax({
        method: 'POST',
        url: '/api/classes/' + room_id + '/allocateToUnit/' + unit.id,
        success: function (response) {
            yay(response);

            setTimeout(function () {
                location.reload();
            }, 1000);
        },
        error: function (response) {
            error(response.responseText);
        }
    })
}