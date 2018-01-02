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

        $('#SelectedTime').select2({
            templateResult: function (state) {
                if (state.children) {
                    return $('<span><i class="glyphicon glyphicon-chevron-right"></i> ' + state.text + '</span>');
                } else {
                    return state.text;
                }
            }
        });

        $('body').on('click', '.select2-results__option', function () {
            var $this = $(this);
            $this.find('ul').toggle();
            $this.find('i').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
        });
    }

    return {
        init: init
    };
}(jQuery, commonFunctions));

$(document).ready(function () {
    currencySetting.init();
});