var dataConfiguration = (function ($) {
    var $filterFieldForm;

    function initModule() {
        $filterFieldForm = $('#filterFieldForm');
        $filterFieldForm.on('submit', function (e) {
            e.preventDefault();

            var fieldId = $('#fieldId').val();
            var getAllUrl = $('.field-' + fieldId).data('get-all-url');

            $.ajax({
                type: 'GET',
                dataType: 'html',
                url: commonFunctions.rootURL + getAllUrl,
                success: onGetSuccess,
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function onGetSuccess(response) {
        $('#tableFieldItem').html(response)
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    dataConfiguration.initModule();
});