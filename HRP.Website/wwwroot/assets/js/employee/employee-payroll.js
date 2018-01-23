var employeePayroll = (function ($) {
    function checkRelatedElementsHaveValue(className, element) {
        return $(className).not(element).serializeArray().filter(function (item) {
            return item.value != '';
        }).length > 0;
    };

    function round(value, currency) {
        if (currency === 'VND')
            return Math.round(value / 1000) * 1000;

        return Number(Math.round(value + 'e2') + 'e-2').toFixed(2);
    }

    function calculateBasicSalary() {
        var totalAmountCurrency = $('#BasicSalary_CurrencyCode').val();
        var $basicSalary = $('#BasicSalary_Salary');

        if (totalAmountCurrency === '' || isNaN($basicSalary.val()))
            return;

        var totalAmount = round(Number($basicSalary.val()), totalAmountCurrency);

        if ($basicSalary.val() !== '')
            $basicSalary.val(totalAmount);

        var selectedAutoCalculate = $('input[name="BasicSalary.AutoCalculateOn"]:checked');
        var parent = selectedAutoCalculate.closest('.group-complex');
        var autoCalculateCurrency = $('select.basic-salary-calculate', parent).val();
        if (autoCalculateCurrency === '')
            return;

        $('input[name="BasicSalary.AutoCalculateOn"]:not(:checked)').each(function (i, e) {
            var parent = $(e).closest('.group-complex');
            var currency = $('select.basic-salary-calculate', parent).val();
            if (currency !== '') {
                var $element = $('input.basic-salary-calculate[type="text"]', parent);
                if ($element.val() !== '' && !isNaN($element.val())) {
                    var amount = round($element.val(), currency);
                    var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                    totalAmount -= amount * exchangeRates;
                }
            }
        });

        totalAmount = round(totalAmount * Number($('input[data-from-currency="' + totalAmountCurrency + '"][data-to-currency="' + autoCalculateCurrency + '"]').val()), autoCalculateCurrency);
        $('input.basic-salary-calculate[type="text"]', parent).val(totalAmount);
    };

    function calculateAmountForBanks() {
        var totalAmountCurrency = $('#Allowance_CurrencyCode').val();
        var totalAmount = Number($('#allowance-total-amount').val());

        if (totalAmountCurrency === '')
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
                var $element = $('input.allowance-calculate[type="text"]', parent);
                if ($element.val() !== '' && !isNaN($element.val())) {
                    var amount = Number($element.val());
                    var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                    totalAmount -= amount * exchangeRates;
                }
            }
        });

        totalAmount = round(totalAmount * Number($('input[data-from-currency="' + totalAmountCurrency + '"][data-to-currency="' + autoCalculateCurrency + '"]').val()), autoCalculateCurrency);
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
            if (value == 0) {
                $('.allowance-convert-amount', parent).val('');
            } else {
                $('.allowance-convert-amount', parent).val(value);
            }
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

    function roundingAmount(element) {
        var parent = $(element).parent();
        var $currency = parent.find('select');
        var $amount = parent.find('input');
        if ($currency.val() !== '' && $amount.val() !== '' && !isNaN($amount.val())) {
            $amount.val(round($amount.val(), $currency.val()));
        }
    };

    function init() {
        calculateBasicSalary();
        calculateAllowances();

        $('.tooltipster').tooltipster({
            content: $('#tooltip-content'),
            contentCloning: true,
            theme: 'tooltipster-light',
            interactive: true,
            trigger: 'custom',
            triggerOpen: {
                mouseenter: true,
            },
            triggerClose: {
                click: true,
                mouseleave: true,
                scroll: true
            },
            functionReady: function (instance, helper) {
                $('input[type="checkbox"]', helper.tooltip).on('change', function (e) {
                    if (e.target.checked) {
                        setTimeout(function () {
                            $('.tooltipster').tooltipster('destroy')
                        }, 500);
                    }
                });
            }
        });

        $('input[name="BasicSalary.AutoCalculateOn"]').on('change', function () {
            if (this.checked) {
                var parent = $(this).closest('.group-complex');
                $('input.basic-salary-calculate[readonly]').removeAttr('readonly');
                $('input.basic-salary-calculate', parent).attr('readonly', 'readonly');
            }
        }).change();

        $('input[name="Allowance.AutoCalculateOn"]').on('change', function () {
            if (this.checked) {
                var parent = $(this).closest('.group-complex');
                $('input.allowance-calculate[readonly]').removeAttr('readonly');
                $('input.allowance-calculate', parent).attr('readonly', 'readonly');
            }
        }).change();

        if ($('#CanUpdate').val() !== 'True') {
            $('select, input', '#payroll-form').attr('disabled', 'disabled');
            return;
        }

        $('.basic-salary-calculate').on('change', function (e) {
            roundingAmount(e.target);
            calculateBasicSalary();
        });

        $('.allowance-calculate').on('change', function (e) {
            roundingAmount(e.target);
            calculateAmountForBanks();
        });

        $('.allowance-detail-calculate').on('change', function (e) {
            roundingAmount(e.target);

            var allowanceCurrency = $('#Allowance_CurrencyCode').val();
            if (allowanceCurrency === '') {
                return;
            }

            convertToAllowanceCurrency($(this), allowanceCurrency);
            calculateTotalAllowance(allowanceCurrency);
        });

        $('.deduction-calculate').on('change', function (e) {
            roundingAmount(e.target);
        });

        $('#Allowance_CurrencyCode').on('change', function () {
            $('.allowance-total-currency:disabled').val(this.value);
            calculateAllowances();
        });

        $('input[name="BasicSalary.WorkingDays"], #duty-meal-per-day').on('change', function (e) {
            var workingDays = $('input[name="BasicSalary.WorkingDays"]:checked').val();
            $('#working-days').val(workingDays);
            $('#Allowance_DutyMealAmount').val(workingDays * $('#duty-meal-per-day').val()).change();
        });

        $('.calendar input').datetimepicker({
            format: 'MM/DD/YYYY',
            useCurrent: false,
            allowInputToggle: true
        });

        $('.bank-type-select').on('change', function () {
            $(this).siblings('.select2.bank-account-select').val('').trigger('change')
        });

        $('.number:enabled').on('keypress', function (e) {
            if ((e.target.value.indexOf('.') >= 0 && e.which === 46) || (e.which !== 46 && (e.which < 48 || e.which > 57))) {
                return false;
            }
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
            placeholder: '',
            minimumResultsForSearch: Infinity,
        });

        $.validator.addMethod('greaterthanzero', function (value, element, param) {
            if (!element.readOnly)
                return value === '' || Number(value) > 0;
            else
                return true;
        }, $('#greater-than-zero-message').val());

        $.validator.addMethod('invalidsetting', function (value, element, param) {
            if (element.readOnly)
                return value === '' || Number(value) > 0;
            else
                return true;
        });

        var classNames = ['basic-salary-ebanking', 'basic-salary-directbanking1', 'basic-salary-directbanking2',
            'basic-salary-cash', 'allowance-directbanking1', 'allowance-directbanking2', 'allowance-cash'];

        classNames.forEach(function (className) {
            $.validator.addClassRules(className, {
                required: function (element) {
                    return checkRelatedElementsHaveValue('.' + className, element);
                }
            });
        });

        $.validator.addClassRules('number', {
            greaterthanzero: true
        });

        var validator = $('#payroll-form').validate({
            debug: true,
            submitHandler: function (form) {
                form.submit();
            },
            onkeyup: false,
            focusInvalid: false,
            errorClass: 'message',
            wrapper: 'div',
            errorLabelContainer: '#validation-summary',
            showErrors: function (errorMap, errorList) {
                this.defaultShowErrors();
                if ($('#validation-summary div').length > 0) {
                    $('#validation-summary').removeClass('hidden');
                } else {
                    $('#validation-summary').addClass('hidden');
                }
            },
            highlight: function (element, errorClass) {
                var $element = $(element);
                $element.addClass('input-validation-error');

                if ($element.hasClass('select2'))
                    $element.next().find('.select2-selection').addClass('input-validation-error').addClass('control');
            },
            unhighlight: function (element, errorClass, validClass) {
                var $element = $(element);
                $element.removeClass('input-validation-error').addClass(validClass);

                if ($element.hasClass('select2'))
                    $element.next().find('.select2-selection').removeClass('input-validation-error control').removeClass(validClass);
            },
            rules: {
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
                }
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