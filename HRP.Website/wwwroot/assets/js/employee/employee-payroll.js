var employeePayroll = (function ($) {
    function init() {
        $('form').validate({
            submitHandler: function (form) {
                form.submit();
            },
            wrapper: 'li',
            errorLableContainer: '#validation-summary ul',
            highlight: function (element, errorClass) {
                $(element).addClass('')
            }
        });
    }

    return {
        init: init
    }
}(jQuery));

$(document).ready(function () {
    employeePayroll.init();
});