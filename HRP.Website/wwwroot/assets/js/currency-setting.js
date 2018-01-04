var currencySetting = (function ($, commonFunctions) {
    function round(value) {
        return Number(Math.round(value + 'e5') + 'e-5');
    }

    function init() {
        var baseCurrency = $('#base-currency').val();

        $('#currency-form').validate({
            errorClass: "text-danger",
            errorElement: "span",
        });

        if ($('#CanUpdate').val() === 'True') {
            $('.rate:enabled').on('change', function () {
                if (!this.checkValidity())
                    return;

                var target = $(this);
                var fromCurrency = target.data('from-currency');
                var toCurrency = target.data('to-currency');
                var value = round(target.val());

                if (toCurrency === baseCurrency) {
                    if (isNaN(value)) {
                        $('.rate:disabled[data-from-currency="' + fromCurrency + '"]').val('');
                        $('.rate:disabled[data-to-currency="' + fromCurrency + '"]').val('');
                        return;
                    }

                    $('.rate:disabled[data-from-currency="' + fromCurrency + '"]').val(function () {
                        var toCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + $(this).data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                        if (toCurrencyOnBaseCurrency === '' || toCurrencyOnBaseCurrency === '0')
                            return;

                        return round(value / toCurrencyOnBaseCurrency);
                    });

                    $('.rate:disabled[data-to-currency="' + fromCurrency + '"]').val(function () {
                        var fromCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                        if (fromCurrencyOnBaseCurrency === '' || fromCurrencyOnBaseCurrency === '0')
                            return;

                        return round(fromCurrencyOnBaseCurrency / value);
                    });

                    $('.rate:enabled[data-from-currency="' + fromCurrency + '"]').not(target).each(function (index, element) {
                        var $element = $(element);
                        if ($element.val() === '' || $element.val() === '0')
                            return;

                        $('.rate:disabled[data-from-currency="' + $element.data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val(round(value / $element.val()));
                    });
                } else {
                    var toCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + toCurrency + '"][data-to-currency="' + baseCurrency + '"]');
                    var fromCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + fromCurrency + '"][data-to-currency="' + baseCurrency + '"]').val();

                    if (isNaN(value) || fromCurrencyOnBaseCurrency === '' || fromCurrencyOnBaseCurrency === '0') {
                        $('.rate:disabled[data-to-currency="' + toCurrency + '"]').val('');
                        $('.rate:disabled[data-from-currency="' + toCurrency + '"]').val('');
                        return;
                    }

                    toCurrencyOnBaseCurrency.val(function () {
                        return round(fromCurrencyOnBaseCurrency / value);
                    });

                    $('.rate:disabled[data-to-currency="' + toCurrency + '"]').val(function () {
                        var fromCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                        if (fromCurrencyOnBaseCurrency === '' || fromCurrencyOnBaseCurrency === '0')
                            return;

                        return round(fromCurrencyOnBaseCurrency / toCurrencyOnBaseCurrency.val());
                    });

                    $('.rate:disabled[data-from-currency="' + toCurrency + '"]').not(toCurrencyOnBaseCurrency).val(function () {
                        var fromCurrencyOnBaseCurrency = $('.rate[data-from-currency="' + $(this).data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();

                        if (fromCurrencyOnBaseCurrency === '' || fromCurrencyOnBaseCurrency === '0')
                            return;

                        return round(toCurrencyOnBaseCurrency.val() / fromCurrencyOnBaseCurrency);
                    });
                }

                target.val(value);
            });

            $('#btn-save').on('click', function () {
                var data = [];

                $('.rate').each(function (index, e) {
                    var element = $(e);
                    data.push({
                        fromCurrency: element.data('from-currency'),
                        toCurrency: element.data('to-currency'),
                        exchangeRate: element.val(),
                    });
                });

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'CurrencySetting/Save',
                    data: {
                        selectedTime: $("#SelectedTime").val(),
                        exchangeRates: data
                    },
                    success: function (response) {
                        if (response.isSuccessful) {
                            commonFunctions.showSuccessMessage('Update successfully!');
                        } else {
                            commonFunctions.showErrorMessage(response.Message);
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
            scrollY: 355,
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            fixedColumns: true
        });

        var calendar = $('.calendar');
        calendar.datetimepicker({
            viewMode: 'months',
            format: 'MM-YYYY',
            minDate: $('#SelectedTime').data('min-month')
        });

        calendar.on('click', function () {
            $(this).data("DateTimePicker").toggle();
        }).on('dp.show', function () {
            $(this).find('.bootstrap-datetimepicker-widget').css('top', '32px');
        });
    }

    return {
        init: init
    };
}(jQuery, commonFunctions));

$(document).ready(function () {
    currencySetting.init();
});