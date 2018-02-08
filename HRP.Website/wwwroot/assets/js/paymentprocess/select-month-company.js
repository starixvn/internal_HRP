var selectMonthCompany = (function ($) {
    function init() {
        $('.datetimepicker').datetimepicker({
            format: 'MM-YYYY',
            useCurrent: false
        });

        $('.select2').select2({
            minimumResultsForSearch: Infinity
        });

        $('.icon-calendar').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').toggle();
        });
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    selectMonthCompany.init();
});