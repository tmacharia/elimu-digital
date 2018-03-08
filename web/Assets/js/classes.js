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
