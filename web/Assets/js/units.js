(function () {
    var unit = {};

    

    $('#unitBtnSubmit').click(function (e) {
        //e.preventDefault();

        if (_proc === 'Assign Lec') {
            //pushAssignLec();
        }
        else {
            postUnit();
        }
        
    })

    $('#allocateUnitForm').submit(function (e) {
        e.preventDefault();

        pushAllocateUnit();
    })
})

function postUnit() {
    unit = {};

    unit.Name = $('#unitName').val();

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