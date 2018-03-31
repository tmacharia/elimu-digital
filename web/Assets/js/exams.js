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
