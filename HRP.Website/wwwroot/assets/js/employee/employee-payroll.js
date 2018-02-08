var employeePayroll = (function ($) {
    function checkRelatedElementsHaveValue(className, element) {
        return $(className).not(element).serializeArray().filter(function (item) {
            return item.value !== '';
        }).length > 0;
    };

    BigNumber.prototype.roundWithCurrency = function (dp, currency) {
        if (currency === 'VND')
            return this.div(1000).round().mul(1000);

        return this.round(dp);
    };

    BigNumber.prototype.toStringWithCurrency = function (currency) {
        if (currency === 'VND')
            return this.toString();

        return this.toFixed(2);
    };

    function checkCurrencyAndAmountChange() {
        var $basicSalary = $('#BasicSalary_Salary');
        var oldValue = $basicSalary.data('old-value');
        var $currency = $('#BasicSalary_CurrencyCode');
        var oldCurrency = $currency.data('old-value');
        return (oldValue !== '' && Number($basicSalary.val()) !== Number(oldValue)) || (oldCurrency !== '' && oldCurrency !== $currency.val());
    }

    function calculateBasicSalary() {
        var totalAmountCurrency = $('#BasicSalary_CurrencyCode').val();
        var $basicSalary = $('#BasicSalary_Salary');

        if (totalAmountCurrency === '' || isNaN($basicSalary.val()))
            return;

        var totalAmount = new BigNumber(Number($basicSalary.val())).roundWithCurrency(2, totalAmountCurrency);

        if ($basicSalary.val() !== '')
            $basicSalary.val(totalAmount.toStringWithCurrency(totalAmountCurrency));

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
                    var amount = new BigNumber($element.val());
                    var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                    totalAmount = totalAmount.minus(amount.mul(exchangeRates));
                }
            }
        });

        totalAmount = totalAmount.mul(Number($('input[data-from-currency="' + totalAmountCurrency + '"][data-to-currency="' + autoCalculateCurrency + '"]').val())).roundWithCurrency(2, autoCalculateCurrency);

        var $autoCalculatedElement = $('input.basic-salary-calculate[type="text"]', parent);
        $autoCalculatedElement.val(totalAmount.toStringWithCurrency(autoCalculateCurrency));
        if ($('#payroll-form').data('validator') && $autoCalculatedElement.hasClass('input-validation-error')) {
            $autoCalculatedElement.valid();
        }
    };

    function calculateAmountForBanks() {
        var totalAmountCurrency = $('#Allowance_CurrencyCode').val();
        var totalAmount = new BigNumber($('#allowance-total-amount').val());

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
                    var amount = new BigNumber($element.val());
                    var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                    totalAmount = totalAmount.minus(amount.mul(exchangeRates));
                }
            }
        });

        totalAmount = totalAmount.mul($('input[data-from-currency="' + totalAmountCurrency + '"][data-to-currency="' + autoCalculateCurrency + '"]').val()).roundWithCurrency(2, autoCalculateCurrency);

        var $autoCalculatedElement = $('input.allowance-calculate[type="text"]', parent);
        $autoCalculatedElement.val(totalAmount.toStringWithCurrency(autoCalculateCurrency));
        if ($('#payroll-form').data('validator') && $autoCalculatedElement.hasClass('input-validation-error')) {
            $autoCalculatedElement.valid();
        }
    };

    function calculateTotalAllowance(allowanceCurrency) {
        var totalAmount = new BigNumber(0);
        if (allowanceCurrency === '') {
            $('#allowance-total-amount').val('');
            return;
        }

        $('.allowance-convert-amount').each(function (i, e) {
            totalAmount = totalAmount.plus(Number($(e).val()));
        });

        $('#allowance-total-amount').val(totalAmount.roundWithCurrency(2, allowanceCurrency).toStringWithCurrency(allowanceCurrency));
        calculateAmountForBanks();
    }

    function convertToAllowanceCurrency(element, allowanceCurrency) {
        var parent = element.closest('.allowance-detail');
        var currency = $('select.allowance-detail-calculate', parent).val();
        var amount = $('input.allowance-detail-calculate', parent).val();
        if (currency !== '' && !isNaN(amount) && amount !== '') {
            amount = new BigNumber(amount);
            var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + allowanceCurrency + '"]').val());
            var value = amount.mul(exchangeRates).roundWithCurrency(2, allowanceCurrency);
            $('.allowance-convert-amount', parent).val(value.toStringWithCurrency(allowanceCurrency));
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
            var number = new BigNumber($amount.val());
            var currency = $currency.val();
            $amount.val(number.roundWithCurrency(2, currency).toStringWithCurrency(currency));
        }
    };

    function validatedInvalidRelatedElements(element) {
        var parent = $(element).closest('.group-complex');
        var validateElements = $('input.input-validation-error, select.input-validation-error').not(element);
        if (validateElements.length > 0) {
            validateElements.valid();
        }
    }

    function getAddBankAccountModal(employeeId, selectedBankType) {
        var createBankAccountInPayrollUrl = $("#create-bank-account-in-payroll-url").val();
        $.ajax({
            url: createBankAccountInPayrollUrl +
                "?employeeId=" +
                employeeId +
                "&" +
                "bankType=" +
                selectedBankType,
            success: function(data) {
                $("#dynamic-modals").html(data);
                applyClientSideInitForAddBankAccountModal();
                $("#dynamic-modals .modal").modal();
            }
        });
    }

    function applyClientSideInitForAddBankAccountModal() {
        $("#dynamic-modals select").select2({
            width: "100%",
            minimumResultsForSearch: Infinity
        });

        bankAccountForm.init();

        var form = $("#dynamic-modals form")
            .removeData("validator") /* added by the raw jquery.validate plugin */
            .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

        $.validator.unobtrusive.parse(form);
    }

    function assignAddBankAccountButton() {
        $("form.payroll").on("click",
            ".add-account",
            function() {
                if ($(this).hasClass("disabled")) {
                    return;
                }

                var selectedBankType = $(this).parent().find(".bank-type-select").val();
                var employeeId = $("#EmployeeId").val();
                getAddBankAccountModal(employeeId, selectedBankType);
            });
    }

    function init() {
        calculateBasicSalary();
        calculateAllowances();
        assignAddBankAccountButton();

        $('.tooltipster').tooltipster({
            content: $('#tooltip-content'),
            contentCloning: true,
            theme: 'tooltipster-light',
            interactive: true,
            trigger: 'custom',
            triggerOpen: {
                mouseenter: true
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
                            $('.tooltipster').tooltipster('destroy');
                        }, 500);
                    }
                });
            }
        });

        $('.bank-type-select').on('change', function (e) {
            $(this).siblings('.bank-account-select').val('').trigger('change.select2');
            validatedInvalidRelatedElements(e.target);

            if (this.value == 2 || this.value == 1) {
                $(this).siblings('.add-account').removeClass('disabled');
            } else {
                $(this).siblings('.add-account').addClass('disabled');
            }
        });

        $('.bank-account-select').on('change', function (e) {
            validatedInvalidRelatedElements(e.target);
        });

        $('.select2').select2({
            allowClear: true,
            placeholder: '',
            minimumResultsForSearch: Infinity,
        }).on('select2:unselecting', function () {
            $(this).data('unselecting', true);
        }).on('select2:opening', function (e) {
            if ($(this).data('unselecting')) {
                $(this).removeData('unselecting');
                e.preventDefault();
            }
        }).on('change', function () {
            $(this).click();
        });

        $('.select2.control.without-label').data('select2').$container.addClass('control without-label');

        $('.select2-ajax').select2({
            ajax: {
                url: $('#get-bank-acount-url').val(),
                data: function (params) {
                    var query = {
                        employeeId: $('#EmployeeId').val(),
                        bankTypeId: $(this).siblings('.bank-type-select').val()
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
            minimumResultsForSearch: Infinity,
        }).on('select2:unselecting', function () {
            $(this).data('unselecting', true);
        }).on('select2:opening', function (e) {
            if ($(this).data('unselecting')) {
                $(this).removeData('unselecting');
                e.preventDefault();
            }
        }).on('change', function () {
            $(this).click();
        });

        if ($('#CanCreateBankAccount').val() !== 'True'){
            $('.add-account').addClass('disabled');
        }

        if ($('#CanUpdate').val() !== 'True') {
            $('select, input', '#payroll-form').attr('disabled', 'disabled');
            return;
        }

        $('.add-account:not(.disabled)').each(function (index, element) {
            var $element = $(element);
            var bankType = $element.siblings('.bank-type-select').val();
            if (bankType == 3 || bankType == '') {
                $element.addClass('disabled');
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

        $('.basic-salary-calculate').on('change', function (e) {
            validatedInvalidRelatedElements(e.target);
            roundingAmount(e.target);
            calculateBasicSalary();
        });

        $('.allowance-calculate').on('change', function (e) {
            validatedInvalidRelatedElements(e.target);
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

        if ($('#BasicSalary_InitFirstTime').val() === 'True') {
            $('#BasicSalary_EffectiveDate, #BasicSalary_ReasonId').attr('disabled', 'disabled');
        } else {
            $('.calendar input').datetimepicker({
                format: 'MM/DD/YYYY',
                useCurrent: false,
                allowInputToggle: true
            });

            $('.icon-calendar').on('click', function () {
                $(this).siblings('input').data('DateTimePicker').toggle();
            });
        }

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

        var $form = $('#payroll-form');
        $form.removeData('validator');
        $form.removeData('unobtrusiveValidation');

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

        var validator = $form.validate({
            debug: true,
            onkeyup: false,
            focusInvalid: false,
            errorClass: 'message',
            wrapper: 'div',
            ignore: 'input[type=hidden]',
            errorLabelContainer: '#validation-summary',
            showErrors: function (errorMap, errorList) {
                this.defaultShowErrors();
                if ($('#validation-summary div').length > 0) {
                    $('#validation-summary').removeClass('hidden');
                } else {
                    $('#validation-summary').addClass('hidden');
                }
            },
            highlight: function (element, errorClass, validClass) {
                var $element = $(element);
                $element.addClass('input-validation-error').removeClass(validClass);

                if ($element.hasClass('select2') || $element.hasClass('select2-ajax'))
                    $element.next().find('.select2-selection').addClass('control input-validation-error');
            },
            unhighlight: function (element, errorClass, validClass) {
                var $element = $(element);
                $element.removeClass('input-validation-error').addClass(validClass);

                if ($element.hasClass('select2') || $element.hasClass('select2-ajax'))
                    $element.next().find('.select2-selection').removeClass('input-validation-error control');
            },
            rules: {
                'BasicSalary.ReasonId': {
                    required: function (element) {
                        return checkCurrencyAndAmountChange();
                    }
                },
                'BasicSalary.EffectiveDate': {
                    required: function (element) {
                        return checkCurrencyAndAmountChange();
                    }
                }
            }
        });
    }

    return {
        init: init,
        onSaveBegin: function () {
            $('#btn-save').button('loading');
        },
        onSaveSuccess: function (response, status) {
            if (response.isSuccessful) {
                $('.msg-success').removeClass('hidden');
                setTimeout(function () {
                    window.location.href = response.redirectTo;
                }, 5000);
            } else {
                if (response.message) {
                    commonFunctions.showErrorMessage($('.msg-error'), response.message);
                }
            }
        },
        onSaveComplete: function () {
            $('#btn-save').button('reset');
        }
    };
}(jQuery));

$(document).ready(function () {
    employeePayroll.init();
});