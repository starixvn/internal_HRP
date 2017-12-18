$(document).ready(function () {
    $('#showPassword').on('click', function () {
        var value = $(this).prop("checked");
        if (value) {
            $('input[type=password').attr("type", "text");
        } else {
            $('input[type=text').attr("type", "password");
        }
    });
})