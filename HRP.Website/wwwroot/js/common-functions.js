var commonFunctions = (function ($) {
    var timer,
        rootURL,
        pagerRanges = [5, 50, 100],
        noDataOnTableMsg = 'There is no data.';

    function showErrorMessage($element, message) {
        $element.text(message).removeClass('hidden');

        clearTimeout(timer);
        timer = setTimeout(function () {
            $element.text('').addClass('hidden');
        }, 3000);
    }

    function showSuccessMessage($element, message) {
        clearTimeout(timer);

        if (message) {
            $element.text(message).removeClass('hidden');

            timer = setTimeout(function () {
                $element.text('').addClass('hidden');
            }, 3000);
        } else {
            $element.removeClass('hidden');

            timer = setTimeout(function () {
                $element.addClass('hidden');
            }, 3000);
        }
    }

    function showHideValidationSummary(targetNode) {
        var callback = function () {
            if ($('.field-validation-error', targetNode).length > 0) {
                $(targetNode).removeClass('hidden');
            } else {
                $(targetNode).addClass('hidden');
            }
        };

        var config = { childList: true, subtree: true };
        var observer = new MutationObserver(callback);
        observer.observe(targetNode, config);
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


    function disableEnterKeySubmit($form) {
        $form.find('input').on('keypress', function (e) {
            return e.keyCode !== 13;
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
        showHideValidationSummary: showHideValidationSummary,
        initDatetimepicker: initDatetimepicker,
        getUrlParameter: getUrlParameter,
        registerEventGoBack: registerEventGoBack,
        noDataOnTableMsg: noDataOnTableMsg,
        disableEnterKeySubmit: disableEnterKeySubmit

    };
}(jQuery));

$(document).ready(function () {
    commonFunctions.initModule();
});