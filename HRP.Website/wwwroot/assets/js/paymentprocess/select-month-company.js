var selectMonthCompany = (function ($) {
    function init() {
        var maxdate = $('#SelectedTime').data('maxdate');
        $('.datetimepicker').datetimepicker({
            format: 'MM-YYYY',
            maxDate: maxdate,
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