var employeePayroll = (function ($) {
    function checkRelatedElementsHaveValue(className, element) {
        return $(className).not(element).serializeArray().filter(function (item) {
            return item.value != '';
        }).length > 0;
    };

    function round(value, currency) {
        if(currency === 'VND')
            return Math.round(value / 1000) * 1000;

        return Number(Math.round(value + 'e2') + 'e-2')
    }

    function calculateBasicSalary() {
        var totalAmountCurrency = $('#BasicSalary_CurrencyCode').val();
        var totalAmount = Number($('#BasicSalary_Salary').val());

        if (totalAmountCurrency === '' || totalAmount <= 0)
            return;

        totalAmount = round(totalAmount, totalAmountCurrency);
        $('#BasicSalary_Salary').val(totalAmount);

        var selectedAutoCalculate = $('input[name="BasicSalary.AutoCalculateOn"]:checked');
        var parent = selectedAutoCalculate.closest('.group-complex');
        var autoCalculateCurrency = $('select.basic-salary-calculate', parent).val();
        if (autoCalculateCurrency === '')
            return;

        $('input[name="BasicSalary.AutoCalculateOn"]:not(:checked)').each(function (i, e) {
            var parent = $(e).closest('.group-complex');
            var currency = $('select.basic-salary-calculate', parent).val();
            if (currency !== '') {
                var amount = Number($('input.basic-salary-calculate[type="text"]', parent).val());
                var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                totalAmount -= amount * exchangeRates;
            }
        });

        totalAmount = round(totalAmount, autoCalculateCurrency);
        $('input.basic-salary-calculate[type="text"]', parent).val(totalAmount);
    };

    function init() {
        calculateBasicSalary();

        $('input[name="BasicSalary.AutoCalculateOn"]').on('change', function () {
            var parent = $(this).closest('.group-complex');
            $('input.basic-salary-calculate').removeAttr('readonly');
            $('input.basic-salary-calculate', parent).attr('readonly', 'readonly');
        }).change();

        $('.basic-salary-calculate').on('change', function () {
            calculateBasicSalary();
        });

        $('.calendar').datetimepicker({
            format: 'DD/MM/YYYY',
            useCurrent: false,
            allowInputToggle : true
        });

        $.validator.addMethod('greaterThanZero', function (value, element, param) {
            return value !== '' && Number(value) > 0;
        });

        $('form').validate({
            debug: true,
            submitHandler: function (form) {
                form.submit();
            },
            wrapper: 'li',
            errorContainer: '#validation-summary',
            errorLabelContainer: '#validation-summary ul',
            highlight: function (element, errorClass) {
                $(element).addClass('input-validation-error');
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass('input-validation-error').addClass(validClass);
            },
            rules: {
                'BasicSalary.Salary': {
                    required: true,
                },
                'BasicSalary.ReasonId': {
                    required: function (element) {
                        var basicSalary = $('#BasicSalary_Salary');
                        var oldValue = basicSalary.data('old-value');
                        return oldValue != '' && basicSalary.val() != oldValue;
                    }
                },
                'BasicSalary.EffectiveDate': {
                    required: function (element) {
                        var basicSalary = $('#BasicSalary_Salary');
                        var oldValue = basicSalary.data('old-value');
                        return oldValue != '' && basicSalary.val() != oldValue;
                    }
                },
                'BasicSalary.EbankingCurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-ebanking', element);
                    }
                },
                'BasicSalary.EbankingAmount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-ebanking', element);
                    },
                },
                'BasicSalary.EbankingAccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-ebanking', element);
                    }
                },
                'BasicSalary.DirectBanking1CurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking1', element);
                    }
                },
                'BasicSalary.DirectBanking1Amount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking1', element);
                    }
                },
                'BasicSalary.DirectBanking1BankTypeId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking1', element);
                    }
                },
                'BasicSalary.DirectBanking1AccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking1', element);
                    }
                },
                'BasicSalary.DirectBanking2CurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking2', element);
                    }
                },
                'BasicSalary.DirectBanking2Amount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking2', element);
                    }
                },
                'BasicSalary.DirectBanking2BankTypeId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking2', element);
                    }
                },
                'BasicSalary.DirectBanking2AccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-directbanking2', element);
                    }
                },
                'BasicSalary.CashCurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-cash', element);
                    }
                },
                'BasicSalary.CashAmount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-cash', element);
                    }
                },
                'BasicSalary.CashBankTypeId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-cash', element);
                    }
                },
                'BasicSalary.CashAccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-cash', element);
                    }
                },
            }
        });
    }

    return {
        init: init
    }
}(jQuery));

$(document).ready(function () {
    employeePayroll.init();
});