var unit = {};

$('#unitForm').submit(function (e) {
    e.preventDefault();

    postUnit();
})

$('#unitBtnSubmit').click(function (e) {
    e.preventDefault();

    postUnit();
})

function postUnit() {
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