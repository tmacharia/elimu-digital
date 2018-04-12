function onNewBoard() {
    $('#unitSelect').empty();

    $.ajax({
        method: 'GET',
        url: '/api/units/my',
        success: function (response) {
            $('#myunits-loader').hide();

            var o = new Option('-- Select --', '');
            $('#unitSelect').append(o);

            if (response.length > 0) {
                for (i = 0; i < response.length; i++) {
                    AddUnitToOptions(response[i]);
                }
            }
        },
        error: function (response) {
            console.log(response);
        }
    })
}
function AddUnitToOptions(unit) {
    var o = new Option(unit.name + ', '+unit.course, unit.id);
    $('#unitSelect').append(o);
}
function pushNewBoard() {
    var board = {
        id: parseInt($('#unitSelect').val()),
        name: $('#boardName').val()
    }
    loadingBtn('boardBtnSubmit', true);
    $.ajax({
        method: 'POST',
        url: '/api/discussionboards/unit?unitId=' + board.id + '&name=' + board.name,
        success: function (response) {
            yay(response);

            setTimeout(function () {
                location.reload();
            }, 1000);
        },
        error: function (res) {
            if (res.responseText) {
                parseError(res.responseText);
            } else {
                error(res.statusText);
            }
        }
    });
    loadingBtn('boardBtnSubmit', false);
}