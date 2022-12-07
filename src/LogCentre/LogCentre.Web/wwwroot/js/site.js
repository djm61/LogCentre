// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const fadeOutMs = 300;

function hideNotifications() {
    $('#notifications').fadeOut(fadeOutMs,
        function () {
            $('#notifications').empty();
        });
}

function showNotification(text) {
    hideNotifications();
    $('#notifications').html(text);
    $('#notifications').fadeIn();
}

$(document).ready(function () {
    jQueryModalGet = (url, title) => {
        window.console.log('jQueryModalGet() | url[' + url + '], title[' + title + ']');
        try {
            $.ajax({
                type: 'GET',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    $('#form-modal .modal-body').html(res.html);
                    $('#form-modal .modal-title').html(title);
                    $('#form-modal').modal('show');
                },
                error: function (err) {
                    console.log(err)
                }
            })
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
    jQueryModalPost = form => {
        window.console.log('jQueryModalPost() | url[' + form.action + ']');
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#viewAll').html(res.html)
                        $('#form-modal').modal('hide');
                    } else {
                        if (res.errors) {
                            window.console.log('POST Error[' + res.errors + ']');
                            showNotification(res.errors);
                        } else {
                            window.console.log('POST Issue[' + res.html + ']');
                            showNotification(res.html);
                        }
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            })
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
    jQueryModalDelete = form => {
        window.console.log('jQueryModalDelete() | url[' + form.action + ']');
        if (confirm('Are you sure to delete this record ?')) {
            try {
                $.ajax({
                    type: 'POST',
                    url: form.action,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.isValid && res.message) {
                            if (res.IsValid === false) {
                                showNotification(res.message);
                            }
                        }

                        if (res.skip) {
                            location.reload();
                        }

                        $('#viewAll').html(res.html);
                    },
                    error: function (err) {
                        console.log(err)
                    }
                })
            } catch (ex) {
                console.log(ex)
            }
        }
        return false;
    }
});