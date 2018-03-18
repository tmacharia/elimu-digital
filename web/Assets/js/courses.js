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