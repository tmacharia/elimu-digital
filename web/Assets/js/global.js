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

