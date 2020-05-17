// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Asynchronous HTTP request.
function asyncReq(method, url, data, callback) {
    var req = new XMLHttpRequest();
    req.onreadystatechange = callback;

    req.open(method, url, true);

    req.setRequestHeader("Cache-Control", "no-cache");
    req.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    req.send(data);
}

// Basic debouncing with a callback. debounceTime is in ms.
function debounce(callback, debounceTime) {
    var timeoutId;

    return function() {
        if (timeoutId) {
            clearTimeout(timeoutId);
        }
        timeoutId = setTimeout(function() {
            callback();
        }, debounceTime);
    }
}