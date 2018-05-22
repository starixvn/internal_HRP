var currencySetting = (function ($, commonFunctions) {
    var vietNamCurrencyId;

    BigNumber.prototype.roundWithCurrency = function (dp, currency) {
        if (currency == vietNamCurrencyId)
            return this.round(0);

        return this.round(dp);
    };

    function round(value) {
        return Number(Math.round(value + 'e5') + 'e-5').toFixed(5);
    }

    function resizeTableWrapper() {
        var $wrapper = $('.dataTables_wrapper');
        var $currencyTable = $('#currency-table');
        if ($wrapper.width() > $currencyTable.width() + 20) {
            $wrapper.width($currencyTable.width() + 20);
        } else {
            $wrapper.width('100%');
        }
    }

    function initDataTable() {
        $('#currency-table').DataTable({
            info: false,
            ordering: false,
            filter: false,
            scrollY: '355px',
            scrollCollapse: true,
            paging: false,
            scrollX: true,
            fixedColumns: true,
            responsive: true,
            initComplete: resizeTableWrapper
        });

        $(window).resize(resizeTableWrapper);
    }

    function checkPayrollExistedForCurrentMonth() {
        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'CurrencySetting/IsPayrollExistedForCurrentMonth',
            success: function (response) {
                if (response.hasPayroll) {
                    $('#modal #warning-message').text(response.message);
                    $('#modal').modal('show');
                } else {
                    updateCurrencySetting();
                }
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function updateCurrencySetting() {
        var data = [];
        $('.rate-real').each(function (index, e) {
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
    }

    function initModule() {
        vietNamCurrencyId = $('#vndCurrencyId').val()
        var baseCurrency = Number($('#base-currency').val());

        if ($('#CanUpdate').val() === 'True') {
            var $rateToVND = $('.rate[data-to-currency="' + vietNamCurrencyId + '"], .rate-real[type="text"][data-to-currency="' + vietNamCurrencyId + '"]');
            $rateToVND.inputmask({
                alias: 'numeric',
                autoGroup: true,
                digits: 0,
                placeholder: '',
                groupSeparator: ',',
                autoUnmask: true,
                allowMinus: false,
                removeMaskOnSubmit: true
            })

            $('.rate-real[type="text"]').not($rateToVND).inputmask({
                alias: 'numeric',
                groupSeparator: ',',
                autoGroup: true,
                digits: 5,
                digitsOptional: true,
                placeholder: '',
                allowMinus: false,
                autoUnmask: true,
                removeMaskOnSubmit: true
            });

            $('.rate-real[type="text"]')
                .on('change', function () {
                    var target = $(this);
                    var fromCurrency = target.data('from-currency');
                    var toCurrency = target.data('to-currency');
                    var value = new BigNumber(Number(target.val()));

                    if (toCurrency === baseCurrency) {
                        if (value.equals(0)) {
                            $('.rate:disabled[data-from-currency="' + fromCurrency + '"]').val('');
                            $('.rate-real:hidden[data-from-currency="' + fromCurrency + '"]').val('');

                            if (fromCurrency !== toCurrency) {
                                $('.rate:disabled[data-to-currency="' + fromCurrency + '"]').val('');
                                $('.rate-real:hidden[data-to-currency="' + fromCurrency + '"]').val('');
                            }

                            return;
                        }

                        $('.rate:disabled[data-from-currency="' + fromCurrency + '"]').val(function () {
                            var toCurrencyOnBaseCurrency = $('.rate-real[data-from-currency="' + $(this).data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                            if (toCurrencyOnBaseCurrency === '' || Number(toCurrencyOnBaseCurrency) === 0)
                                return;

                            var convertValue = value.div(toCurrencyOnBaseCurrency);
                            $('.rate-real[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + $(this).data('to-currency') + '"]').val(convertValue.round(10).toString());

                            return convertValue.roundWithCurrency(5, $(this).data('to-currency')).toFixed(5);
                        });

                        $('.rate:disabled[data-to-currency="' + fromCurrency + '"]').val(function () {
                            var fromCurrencyOnBaseCurrency = $('.rate-real[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                            if (fromCurrencyOnBaseCurrency === '' || Number(fromCurrencyOnBaseCurrency) === 0)
                                return;

                            var convertValue = new BigNumber(fromCurrencyOnBaseCurrency).div(value);

                            $('.rate-real[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + $(this).data('to-currency') + '"]').val(convertValue.round(10).toString());
                            return convertValue.roundWithCurrency(5, $(this).data('to-currency')).toFixed(5);
                        });

                        $('.rate-real[type="text"][data-from-currency="' + fromCurrency + '"]').not(target).each(function (index, element) {
                            var $element = $(element);
                            var elementValue = $element.val();
                            if (elementValue === '' || Number(elementValue) === 0)
                                return;

                            var convertValue = value.div(elementValue);

                            $('.rate:disabled[data-from-currency="' + $element.data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val(convertValue.roundWithCurrency(5, baseCurrency).toString());
                            $('.rate-real[data-from-currency="' + $element.data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val(convertValue.round(10).toString());
                        });
                    } else {
                        var $toCurrencyOnBaseCurrencyDisplay = $('.rate[data-from-currency="' + toCurrency + '"][data-to-currency="' + baseCurrency + '"]');
                        var $toCurrencyOnBaseCurrency = $('.rate-real[data-from-currency="' + toCurrency + '"][data-to-currency="' + baseCurrency + '"]');

                        var fromCurrencyOnBaseCurrency = $('.rate-real[data-from-currency="' + fromCurrency + '"][data-to-currency="' + baseCurrency + '"]').val();

                        if (value.equals(0) || fromCurrencyOnBaseCurrency === '' || Number(fromCurrencyOnBaseCurrency) === 0) {
                            $('.rate:disabled[data-to-currency="' + toCurrency + '"]').val('');
                            $('.rate-real:hidden[data-to-currency="' + toCurrency + '"]').val('');
                            $('.rate:disabled[data-from-currency="' + toCurrency + '"]').val('');
                            $('.rate-real:hidden[data-from-currency="' + toCurrency + '"]').val('');
                            return;
                        }

                        var convertValue = new BigNumber(fromCurrencyOnBaseCurrency).div(value);
                        $toCurrencyOnBaseCurrencyDisplay.val(convertValue.roundWithCurrency(5, baseCurrency).toFixed(5));

                        convertValue = convertValue.round(10);
                        $toCurrencyOnBaseCurrency.val(convertValue.toString());

                        $('.rate:disabled[data-from-currency="' + toCurrency + '"]').not($toCurrencyOnBaseCurrencyDisplay).val(function () {
                            var toCurrencyOnBaseCurrency = $('.rate-real[data-from-currency="' + $(this).data('to-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                            if (toCurrencyOnBaseCurrency === '' || Number(toCurrencyOnBaseCurrency) === 0)
                                return;

                            var value = convertValue.div(toCurrencyOnBaseCurrency);
                            $('.rate-real[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + $(this).data('to-currency') + '"]').val(value.round(10).toString());

                            return value.roundWithCurrency(5, $(this).data('to-currency')).toFixed(5);
                        });

                        $('.rate:disabled[data-to-currency="' + toCurrency + '"]').val(function () {
                            var fromCurrencyOnBaseCurrency = $('.rate-real[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + baseCurrency + '"]').val();
                            if (fromCurrencyOnBaseCurrency === '' || Number(fromCurrencyOnBaseCurrency) === 0)
                                return;

                            var value = new BigNumber(fromCurrencyOnBaseCurrency).div(convertValue);

                            $('.rate-real[data-from-currency="' + $(this).data('from-currency') + '"][data-to-currency="' + $(this).data('to-currency') + '"]').val(value.round(10).toString());
                            return value.roundWithCurrency(5, $(this).data('to-currency')).toFixed(5);
                        });
                    }

                    target.val(value.toFixed(5));
                });

            $('#btn-save').on('click', function () {
                checkPayrollExistedForCurrentMonth();
            });

            $('#btn-yes').on('click', function () {
                updateCurrencySetting();
                $('#modal').modal('hide');
            });
        }

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
        initModule: initModule,
        initDataTable: initDataTable
    };
}(jQuery, commonFunctions));

$(document).ready(function () {
    currencySetting.initModule();
});

if (document.readyState === 'loading') {
    currencySetting.initDataTable();
}