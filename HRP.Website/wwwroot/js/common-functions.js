var commonFunctions = (function ($) {
    var rootURL,
        pagerRanges = [20, 50, 100],
        noDataOnTableMsg = 'There is no data.';

    function showErrorMessage(message) {
        $('.msg-error').text(message).removeClass('hidden');

        setTimeout(function () {
            $('.msg-error').text('').addClass('hidden');
        }, 5000);
    }

    function showSuccessMessage(message) {
        $('.msg-success').text(message).removeClass('hidden');

        setTimeout(function () {
            $('.msg-success').text('').addClass('hidden');
        }, 5000);
    }

    function showValidationSummary($form) {
        $form.find('.list-message-error').removeClass('hidden');
    }

    function hideValidationSummary($form) {
        $form.find('.list-message-error').addClass('hidden');
    }

    function initDatetimepicker($form) {
        $form.find('input.datetimepicker').each(function () {
            var $this = $(this),
                defaultDate = $this.val(),
                mindate = $this.data('mindate'),
                maxdate = $this.data('maxdate');

            $this.datetimepicker({
                format: 'MMM DD, YYYY',
                minDate: mindate,
                maxDate: maxdate,
                useCurrent: false
            });

            if (defaultDate) {
                defaultDate = moment(defaultDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
                $this.data('DateTimePicker').date(defaultDate);
            } else {
                $this.val('');
            }
        });

        $form.find('.icon-calendar:not(.disabled)').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').show();
        });
    }

    function handleAjaxError(xhr, textStatus, errorThrown) {
        var url;

        switch (xhr.status) {
            case 401:
                url = 'auth/login';
                break;

            case 405:
                url = 'error/405';
                break;

            case 500:
                url = 'error/500';
                break;
        }

        window.location = rootURL + url;
    }

    function getUrlParameter(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    }

    function registerEventGoBack($btnCancel) {
        $btnCancel.on('click', function () {
            window.history.back();
        });
    }

    function initModule() {
        rootURL = $('#RootURL').val();
    }

    return {
        initModule: initModule,
        rootURL: $('#RootURL').val(),
        pagerRanges: pagerRanges,
        handleAjaxError: handleAjaxError,
        showErrorMessage: showErrorMessage,
        showSuccessMessage: showSuccessMessage,
        showValidationSummary: showValidationSummary,
        hideValidationSummary: hideValidationSummary,
        initDatetimepicker: initDatetimepicker,
        getUrlParameter: getUrlParameter,
        registerEventGoBack: registerEventGoBack,
        noDataOnTableMsg: noDataOnTableMsg
    };
}(jQuery));

$(document).ready(function () {
    commonFunctions.initModule();
});