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