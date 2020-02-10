// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function RandomNumber() {
    return Math.floor(Math.random() * 10);
}
function RandomCode() {
    return RandomNumber().toString() + RandomNumber().toString() + RandomNumber().toString() + RandomNumber().toString();
}
function EnterOnlyNumber(event) {
    var key = window.event ? event.keyCode : event.which;
    if (event.keyCode === 8 || event.keyCode === 46 || event.charCode === 44 || event.keyCode === 37 || event.keyCode === 39) { //backspace, canc, comma, frecce
        return true;
    } else if (key < 48 || key > 57) {
        return false;
    } else {
        return true;
    }
}

$(document).ready(function () {

    var clipboard = new ClipboardJS('.clipboard');

    $("script.inject-json").each(function (i, e) {
        var name = $(e).attr("data-name") || "injectedJson";
        var json = $(e).html();
        if (name !== "injectedJson") {
            window[name] = JSON.parse(json);
        } else {
            if (!window[name]) {
                window[name] = new Array();
            }
            window[name].push(JSON.parse(json));
        }
    });

});
