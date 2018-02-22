var currencySetting = (function ($, commonFunctions) {
    function round(value) {
        return Number(Math.round(value + 'e5') + 'e-5').toFixed(5);
    }

    function init() {
        var baseCurrency = Number($('#base-currency').val());

        if ($('#CanUpdate').val() === 'True') {
            $('.rate:enabled, .rate:disabled[data-to-currency="' + baseCurrency + '"]').inputmask()
                .on('change', function () {
                    var target = $(this);
                    var fromCurrency = target.data('from-currency');
                    var toCurrency = target.data('to-currency');
                    var value = round(target.inputmask('unmaskedvalue'));

                    if (toCurrency === baseCurrency) {
                        if (Number(value) === 0) {
                            $('.rate:disabled[data-from-currency="' + fromCurrency + '"]').val('');
                            $('.rate:disabled[data-to-currency="' + fromCurrency + '"]').val('');
                            return;
                        }

                        $('.rate:disabled[data-from-currency="' + fromCurrency + '"]').val(function () {
                            var toCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + $(this).data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').inputmask('unmaskedvalue');
                            if (toCurrencyOnBaseCurrency === '' || Number(toCurrencyOnBaseCurrency) === 0)
                                return;

                            return round(value / toCurrencyOnBaseCurrency);
                        });

                        $('.rate:disabled[data-to-currency="' + fromCurrency + '"]').val(function () {
                            var fromCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + baseCurrency + '"]').inputmask('unmaskedvalue');
                            if (fromCurrencyOnBaseCurrency === '' || Number(fromCurrencyOnBaseCurrency) === 0)
                                return;

                            return round(fromCurrencyOnBaseCurrency / value);
                        });

                        $('.rate:enabled[data-from-currency="' + fromCurrency + '"]').not(target).each(function (index, element) {
                            var $element = $(element);
                            var elementValue = $element.inputmask('unmaskedvalue');
                            if (elementValue === '' || Number(elementValue) === 0)
                                return;

                            $('.rate:disabled[data-from-currency="' + $element.data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val(round(value / elementValue));
                        });
                    } else {
                        var toCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + toCurrency + '"][data-to-currency="' + baseCurrency + '"]');
                        var fromCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + fromCurrency + '"][data-to-currency="' + baseCurrency + '"]').inputmask('unmaskedvalue');

                        if (isNaN(value) || fromCurrencyOnBaseCurrency === '' || Number(fromCurrencyOnBaseCurrency) === 0) {
                            $('.rate:disabled[data-to-currency="' + toCurrency + '"]').val('');
                            $('.rate:disabled[data-from-currency="' + toCurrency + '"]').val('');
                            return;
                        }

                        toCurrencyOnBaseCurrency.val(function () {
                            return round(fromCurrencyOnBaseCurrency / value);
                        });

                        toCurrencyOnBaseCurrency.change();
                    }

                    target.val(value);
                });

            $('#btn-save').on('click', function () {
                var data = [];
                $('.rate').each(function (index, e) {
                    var element = $(e);
                    data.push({
                        fromCurrencyId: element.data('from-currency'),
                        toCurrencyId: element.data('to-currency'),
                        exchangeRate: element.val()
                    });
                });

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'CurrencySetting/Edit',
                    data: {
                        selectedTime: $("#SelectedTime").val(),
                        exchangeRates: data,
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', '#currency-form').val()
                    },
                    success: function (response) {
                        if (response.isSuccessful) {
                            commonFunctions.showSuccessMessage($('.msg-success'));
                        } else {
                            commonFunctions.showErrorMessage($('.msg-error'), response.Message);
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            });
        }

        $('#currency-table').DataTable({
            info: false,
            ordering: false,
            filter: false,
            scrollY: '355px',
            scrollCollapse: true,
            paging: false,
            scrollX: true,
            fixedColumns: true,
            responsive: true
        });

        var calendar = $('.datetimepicker');
        calendar.datetimepicker({
            viewMode: 'months',
            format: 'MM-YYYY',
            minDate: $('#SelectedTime').data('min-month')
        });

        $('.icon-calendar').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').toggle();
        });
    }

    return {
        init: init
    };
}(jQuery, commonFunctions));

$(document).ready(function () {
    currencySetting.init();
});