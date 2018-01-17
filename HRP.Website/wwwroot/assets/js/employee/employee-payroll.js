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

    function calculateAmountForBanks() {
        var totalAmountCurrency = $('#Allowance_CurrencyCode').val();
        var totalAmount = Number($('#allowance-total-amount').val());

        if (totalAmountCurrency === '' || totalAmount <= 0)
            return;

        var selectedAutoCalculate = $('input[name="Allowance.AutoCalculateOn"]:checked');
        var parent = selectedAutoCalculate.closest('.group-complex');
        var autoCalculateCurrency = $('select.allowance-calculate', parent).val();
        if (autoCalculateCurrency === '')
            return;

        $('input[name="Allowance.AutoCalculateOn"]:not(:checked)').each(function (i, e) {
            var parent = $(e).closest('.group-complex');
            var currency = $('select.allowance-calculate', parent).val();
            if (currency !== '') {
                var amount = Number($('input.allowance-calculate[type="text"]', parent).val());
                var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                totalAmount -= amount * exchangeRates;
            }
        });

        totalAmount = round(totalAmount, autoCalculateCurrency);
        $('input.allowance-calculate[type="text"]', parent).val(totalAmount);
    };

    function calculateTotalAllowance(allowanceCurrency) {
        var totalAmount = 0;
        if (allowanceCurrency === '') {
            $('#allowance-total-amount').val('');
            return;
        }

        $('.allowance-convert-amount').each(function (i, e) {
            totalAmount += Number($(e).val())
        });

        $('#allowance-total-amount').val(round(totalAmount, allowanceCurrency));
        calculateAmountForBanks();
    }

    function convertToAllowanceCurrency(element, allowanceCurrency) {
        var parent = element.closest('.allowance-detail');
        var currency = $('select.allowance-detail-calculate', parent).val();
        if (currency != '') {
            var amount = Number($('input.allowance-detail-calculate', parent).val());
            var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + allowanceCurrency + '"]').val());
            var value = round(amount * exchangeRates, allowanceCurrency);
            $('.allowance-convert-amount', parent).val(value);
        }
        else {
            $('.allowance-convert-amount', parent).val('');
        }
    }

    function calculateAllowances() {
        var allowanceCurrency = $('#Allowance_CurrencyCode').val();
        if (allowanceCurrency === '') {
            return;
        }

        $('.allowance-detail').each(function (i, e) {
            convertToAllowanceCurrency($(e), allowanceCurrency);
        });
        calculateTotalAllowance(allowanceCurrency);
    }

    function init() {
        calculateBasicSalary();
        calculateAllowances();

        $('.allowance-detail-calculate').on('change', function () {
            var allowanceCurrency = $('#Allowance_CurrencyCode').val();
            if (allowanceCurrency === '') {
                return;
            }

            convertToAllowanceCurrency($(this), allowanceCurrency);
            calculateTotalAllowance(allowanceCurrency);
        });

        $('#Allowance_CurrencyCode').on('change', function () {
            $('.allowance-total-currency:disabled').val(this.value);
            calculateAllowances();
        });

        $('input[name="BasicSalary.WorkingDays"], #duty-meal-per-day').on('change', function (e) {
            debugger;
            var workingDays = $('input[name="BasicSalary.WorkingDays"]:checked').val();
            $('#working-days').val(workingDays);
            $('#Allowance_DutyMealAmount').val(workingDays * $('#duty-meal-per-day').val()).change();
        });

        $('input[name="BasicSalary.AutoCalculateOn"]').on('change', function () {
            var parent = $(this).closest('.group-complex');
            $('input.basic-salary-calculate[readonly]').removeAttr('readonly');
            $('input.basic-salary-calculate', parent).attr('readonly', 'readonly');
        }).change();

        $('input[name="Allowance.AutoCalculateOn"]').on('change', function () {
            var parent = $(this).closest('.group-complex');
            $('input.allowance-calculate[readonly]').removeAttr('readonly');
            $('input.allowance-calculate', parent).attr('readonly', 'readonly');
        }).change();

        $('.number:enabled').inputmask({
            alias: 'decimal'
        })

        $('.allowance-calculate').on('change', function () {
            calculateAmountForBanks();
        });

        $('.basic-salary-calculate').on('change', function () {
            calculateBasicSalary();
        });

        $('.calendar').datetimepicker({
            format: 'DD/MM/YYYY',
            useCurrent: false,
            allowInputToggle : true
        });

        $('.bank-type-select').on('change', function () {
            $(this).siblings('.select2.bank-account-select').val('').trigger('change')
        });

        $('.select2').select2({
            ajax: {
                url: $('#get-bank-acount-url').val(),
                data: function (params) {
                    var query = {
                        employeeId: $('#EmployeeId').val(),
                        bankTypeId: $(this).siblings('.bank-type-select').val()
                    }

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
            placeholder: 'Select a bank account',
            minimumResultsForSearch: Infinity,
        });

        $.validator.addMethod('greaterThanZero', function (value, element, param) {
            return value === '' || Number(value) > 0;
        });

        var validator = $('form').validate({
            debug: true,
            submitHandler: function (form) {
                form.submit();
            },
            errorClass: 'message',
            wrapper: 'span',
            errorLabelContainer: '#validation-summary',
            showErrors: function (errorMap, errorList) {
                if (errorList.length > 0) {
                    $('#validation-summary').removeClass('hidden');
                } else {
                    $('#validation-summary').addClass('hidden');
                }
                this.defaultShowErrors();
            },
            highlight: function (element, errorClass) {
                var $element = $(element);
                $element.addClass('input-validation-error');

                if ($element.hasClass('select2'))
                    $element.next().find('.select2-selection').addClass('input-validation-error').addClass('control').removeClass(validClass);
            },
            unhighlight: function (element, errorClass, validClass) {
                var $element = $(element);
                $element.removeClass('input-validation-error').addClass(validClass);

                if ($element.hasClass('select2'))
                    $element.next().find('.select2-selection').removeClass('input-validation-error control').removeClass(validClass);
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
                'BasicSalary.CashAccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.basic-salary-cash', element);
                    }
                },
                'Allowance.DirectBanking1CurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking1', element);
                    }
                },
                'Allowance.DirectBanking1Amount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking1', element);
                    }
                },
                'Allowance.DirectBanking1BankTypeId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking1', element);
                    }
                },
                'Allowance.DirectBanking1AccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking1', element);
                    }
                },
                'Allowance.DirectBanking2CurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking2', element);
                    }
                },
                'Allowance.DirectBanking2Amount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking2', element);
                    }
                },
                'Allowance.DirectBanking2BankTypeId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking2', element);
                    }
                },
                'Allowance.DirectBanking2AccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-directbanking2', element);
                    }
                },
                'Allowance.CashCurrencyCode': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-cash', element);
                    }
                },
                'Allowance.CashAmount': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-cash', element);
                    }
                },
                'Allowance.CashAccountId': {
                    required: function (element) {
                        return checkRelatedElementsHaveValue('.allowance-cash', element);
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