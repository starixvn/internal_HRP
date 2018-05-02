var monthlyPayroll = (function ($) {
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

        commonFunctions.showHideValidationSummary($('#filterForm').find('.list-message-error').get(0));

        $('.js-btnExport').on('click', function () {
            $('#filterForm').validate().showErrors({
                'CommonError': ''
            });
        });
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    monthlyPayroll.init();
});