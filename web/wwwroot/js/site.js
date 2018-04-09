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
function onSelectUnit4Exam() {
    $('#unitSelect').empty();
    $.ajax({
        method: 'GET',
        url: '/api/units/my',
        success: function (response) {
            $('#units-loader').hide();

            var o = new Option('-- Select --', '');
            $('#unitSelect').append(o);

            if (response.length > 0) {
                for (i = 0; i < response.length; i++) {
                    AddUnitToSetExamOptions(response[i]);
                }
            }
        },
        error: function (response) {
            console.log(response);
            if (response.responseText) {
                parseError(response.responseText);
            } else {
                error(response.statusText);
            }
        }
    })
}
function AddUnitToSetExamOptions(unit) {
    var o = new Option(unit.name, unit.id);
    $('#unitSelect').append(o);
}

function navigateToSetExam() {
    var unit = {};
    unit.id = $('#unitSelect').val();
    var selectedOption = $('#unitSelect option').each(function (index, elem) {
        if ($(elem)[0].value === unit.id) {
            unit.name = $(elem)[0].text;
        }
    });

    window.location = '/exams/set-for/' + slugify(unit.name) + '/' + unit.id;
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
    toastr.success(msg, 'Success');
}
function info(msg) {
    toastr.info(msg, 'Information');
}
function error(msg) {
    toastr.error(msg, 'Error');
}
function warning(msg) {
    toastr.warning(msg, 'Warning', {
        progressBar: true,
        positionClass: 'toast-bottom-right'
    });
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

function slugify(string) {
    return string
      .toString()
      .trim()
      .toLowerCase()
      .replace(/\s+/g, "-")
      .replace(/[^\w\-]+/g, "")
      .replace(/\-\-+/g, "-")
      .replace(/^-+/, "")
      .replace(/-+$/, "");
}

function newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
/**
     * @@param num The number to round off
     * @@param precision The number of decimal places to preserve
     */
function roundOff(number, decimal) {
    var zeros = new String(1.0.toFixed(decimal));
    zeros = zeros.substr(2);
    var mul_div = parseInt("1" + zeros);
    var increment = parseFloat("." + zeros + "01");
    if (((number * (mul_div * 10)) % 10) >= 5)
    { number += increment; }
    return Math.round(number * mul_div) / mul_div;
}
Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}
Date.prototype.addMinutes = function (m) {
    this.setMinutes(this.getMinutes() + m);
    return this;
}
Date.prototype.addSeconds = function (s) {
    this.setSeconds(this.getSeconds() + s);
    return this;
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

function onDeleteUnit(id) {
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
                    deleteUnit(id);
                }
            },
            No: function () {

            }
        }
    })
};

function deleteUnit(id) {
    $.confirm({
        theme: 'supervan',
        content: function () {
            var self = this;

            return $.ajax({
                url: '/api/units/' + id,
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