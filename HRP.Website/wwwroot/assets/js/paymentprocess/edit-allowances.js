var editAllowances = (function ($, commonFunctions) {
    BigNumber.prototype.roundWithCurrency = function (dp, currency) {
        if (currency === '11')
            return this.div(1000).round().mul(1000);

        return this.round(dp);
    };

    BigNumber.prototype.toStringWithCurrency = function (currency) {
        if (currency === '11')
            return this.toString();

        return this.toFixed(2);
    };

    function calculateTotalAddition() {
        var totalAddition = new BigNumber(0);
        $('.addtion.net-allowance').each(function (i, element) {
            totalAddition = totalAddition.plus(Number(element.value));
        });

        $("#total-addition").val(totalAddition.toNumber());
    };

    function calculateTotalDeduction() {
        var totalDeduction = new BigNumber(0);
        $('.deduction.net-allowance').each(function (i, element) {
            totalDeduction = totalDeduction.plus(Number(element.value));
        });

        $("#total-deduction").val(totalDeduction.toNumber());
    };


    function initModule() {
        $('.number:enabled').on('keydown', function (e) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A, Command+A
                (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        });

        $('.select2').select2({
            allowClear: true,
            placeholder: '',
            minimumResultsForSearch: Infinity
        })

        $('.select2-ajax').select2({
            ajax: {
                url: $('#get-bank-acount-url').val(),
                data: function (params) {
                    var query = {
                        employeeId: $('#EmployeeId').val(),
                        bankTypeId: $(this).siblings('select').val()
                    };

                    return query;
                },
                dataType: 'json',
                processResults: function (data) {
                    return {
                        results: data
                    };
                }
            },
            allowClear: true,
            placeholder: '',
            minimumResultsForSearch: Infinity
        })

        $('.unit-amount, .no-of-days').inputmask({
            'alias': 'numeric',
            'autoGroup': true,
            'digits': 2,
            'placeholder': '',
            'allowMinus': false
        })

        $('.bank-type-select').on('change', function (e) {
            $(this).siblings('.bank-account-select').val('').trigger('change.select2');
        });

        $('.addition, .deduction').on('change', function (e) {
            var totalCurrencyId = $('#Allowance_CurrencyId').val();
            var $this = $(this);
            var dataGroup = $this.data('group');
            var $relatedElements = $('[data-group="' + dataGroup + '"]');

            var $unitAmount = $relatedElements.filter('.unit-amount');
            var $currency = $relatedElements.filter('.currency');
            var currencyId = $currency.val();
            var $noOfDays = $relatedElements.filter('.no-of-days');
            var $totalAmount = $relatedElements.filter('.total-amount');  
            var $totalAmountDisplay = $relatedElements.filter('.total-amount-display');
            var $netAllowance = $relatedElements.filter('.net-allowance');
            var $netAllowanceDisplay = $relatedElements.filter('.net-allowance-display');

            if ($unitAmount.val() !== '') {     
                var unitAmount = new BigNumber($unitAmount.val());
                var totalAmount = '';

                if ($noOfDays.val() !== '') {
                    totalAmount = unitAmount.mul($noOfDays.val());
                    $totalAmount.val(totalAmount.toString());
                    $totalAmountDisplay.val(totalAmount.roundWithCurrency(2, currencyId).toStringWithCurrency(currencyId));
                } else {
                    totalAmount = unitAmount
                    $totalAmount.val(totalAmount.toString());
                    $totalAmountDisplay.val(totalAmount.roundWithCurrency(2, currencyId).toStringWithCurrency(currencyId));
                }

                if (currencyId !== '') {
                    var exchangeRates = Number($('input[data-from-currency="' + currencyId + '"][data-to-currency="' + totalCurrencyId + '"]').val());
                    var netAllowance = totalAmount.mul(exchangeRates);
                    $netAllowance.val(netAllowance.toString());
                    $netAllowanceDisplay.val(netAllowance.roundWithCurrency(2, totalCurrencyId).toStringWithCurrency(totalCurrencyId));
                } else {
                    $netAllowance.val('')
                    $netAllowanceDisplay.val('');
                }
            } else {
                $totalAmount.val('');
                $totalAmountDisplay.val('');
                $netAllowance.val('');
                $netAllowanceDisplay.val('');
            }         
        });

        var $editAllowanceForm = $("#edit-allowances-form");
        $editAllowanceForm.removeData("validator").removeData("unobtrusiveValidation");

        $.validator.addMethod('requiredifrelatedfieldhasvalue', function (value, element, params) {
            var $element = $(element);
            var dataGroup = $element.data('group');
            var $relatedElement = $('.no-of-days[data-group="' + dataGroup + '"]');
            if ($relatedElement.val() !== '' && $element.val() === '') {
                return false;
            }

            return true;
        });

        $.validator.unobtrusive.adapters.add('requiredifrelatedfieldhasvalue', [], function (options) {
            options.rules['requiredifrelatedfieldhasvalue'] = [];
            options.messages['requiredifrelatedfieldhasvalue'] = options.message;
        });

        $.validator.unobtrusive.parse($editAllowanceForm);
        commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
    }

    return {
        initModule: initModule
    };
}(jQuery, commonFunctions));

$(document).ready(function () {
    editAllowances.initModule();
});