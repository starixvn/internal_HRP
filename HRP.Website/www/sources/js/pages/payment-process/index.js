import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';
import 'eonasdan-bootstrap-datetimepicker/src/js/bootstrap-datetimepicker';
import 'select2/dist/js/select2';

const selectMonthCompany = (function ($) {
    function init() {
        var maxdate = $('#SelectedTime').data('maxdate');
        $('.datetimepicker').datetimepicker({
            viewMode: 'months',
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