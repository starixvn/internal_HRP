import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';
import 'bootstrap/dist/js/bootstrap';
import 'eonasdan-bootstrap-datetimepicker/src/js/bootstrap-datetimepicker';
import 'select2/dist/js/select2';
import 'tooltipster/src/js/tooltipster';
import 'tooltipster/src/js/plugins/tooltipster/sideTip/tooltipster-sideTip';
import BigNumber from 'bignumber.js';
import 'inputmask/dist/inputmask';
import 'inputmask/dist/jquery.inputmask';
import 'inputmask/dist/bindings/inputmask.binding';
import '../bank-account/bank-account-form';
import rijndaelService from '../../common/rijndael-service';

window.employeePayroll = (function ($) {
    let vndCurrencyId, $plainValues, $form, rijndaelIV, rijndaelKey;

    function checkRelatedElementsHaveValue(className, element) {
        return $(className).not(element).serializeArray().filter(function (item) {
            return item.value !== '';
        }).length > 0;
    }

    BigNumber.prototype.roundWithCurrency = function (dp, currency, roundingThousand = true) {
        if (currency === vndCurrencyId)
            return roundingThousand ? this.div(1000).round().mul(1000) : this.round(0);

        return this.round(dp);
    };

    BigNumber.prototype.toStringWithCurrency = function (currency) {
        if (currency === vndCurrencyId)
            return this.toString();

        return this.toFixed(2);
    };

    function checkCurrencyAndAmountChange(element) {
        var oldElementValue = $(element).data('old-value');
        var $basicSalary = $('#BasicSalary_Salary_PlainValue');
        var oldSalary = $('#BasicSalary_OldSalary_PlainValue').val();
        var $currency = $('#BasicSalary_Salary_CurrencyId');
        var oldCurrency = $('#BasicSalary_OldSalary_CurrencyId').val();

        return (oldSalary !== '' && $basicSalary.val() !== '' && (Number($basicSalary.val()) !== Number(oldSalary) || oldElementValue !== ''))
            || (oldCurrency !== '' && $currency.val() !== '' && ($currency.val() !== oldCurrency || oldElementValue !== ''));
    }

    function calculateAutoBasicSalary() {
        var totalAmountCurrency = $('#BasicSalary_Salary_CurrencyId').val();
        var $basicSalary = $('#BasicSalary_Salary_PlainValue');

        var selectedAutoCalculate = $('input[name="BasicSalary.AutoCalculateOn"]:checked');
        var parent = selectedAutoCalculate.closest('.group-complex');
        var autoCalculateCurrency = $('select.basic-salary-calculate', parent).val();
        var $autoCalculatedElement = $('input.basic-salary-calculate[type="text"]', parent);

        if (totalAmountCurrency === '' || $basicSalary.val() === '' || autoCalculateCurrency === '') {
            $autoCalculatedElement.val('');
            return;
        }  

        var totalAmount = new BigNumber(Number($basicSalary.val()));

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

        totalAmount = totalAmount.mul(Number($('input[data-from-currency="' + totalAmountCurrency + '"][data-to-currency="' + autoCalculateCurrency + '"]').val())).roundWithCurrency(0, autoCalculateCurrency);
        $autoCalculatedElement.val(totalAmount.toString());

        if ($('#payroll-form').data('validator')) {
            $autoCalculatedElement.valid();
        }
    }

    function calculateAutoAllowance() {
        var totalAmountCurrency = $('#Allowance_CurrencyId').val();
        var totalAmount = new BigNumber(Number($('#allowance-total-amount').val()));

        var selectedAutoCalculate = $('input[name="Allowance.AutoCalculateOn"]:checked');
        var parent = selectedAutoCalculate.closest('.group-complex');
        var autoCalculateCurrency = $('select.allowance-calculate', parent).val();
        var $autoCalculatedElement = $('input.allowance-calculate', parent);

        if (autoCalculateCurrency === '' || totalAmountCurrency === '') {
            $autoCalculatedElement.val('');
            return;
        }

        $('input[name="Allowance.AutoCalculateOn"]:not(:checked)').each(function (i, e) {
            var parent = $(e).closest('.group-complex');
            var currency = $('select.allowance-calculate', parent).val();
            if (currency !== '') {
                var $element = $('input.allowance-calculate', parent);
                if ($element.val() !== '' && !isNaN($element.val())) {
                    var amount = new BigNumber($element.val());
                    var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrency + '"]').val());
                    totalAmount = totalAmount.minus(amount.mul(exchangeRates));
                }
            }
        });

        totalAmount = totalAmount.mul($('input[data-from-currency="' + totalAmountCurrency + '"][data-to-currency="' + autoCalculateCurrency + '"]').val()).roundWithCurrency(0, autoCalculateCurrency);

        $autoCalculatedElement.val(totalAmount.toString());
        if ($('#payroll-form').data('validator')) {
            $autoCalculatedElement.valid();
        }
    }

    function calculateTotalAllowance(allowanceCurrency) {
        var totalAmount = new BigNumber(0);
        if (allowanceCurrency === '') {
            $('#allowance-total-amount').val('');
            return;
        }

        $('.allowance-convert-amount').each(function (i, e) {
            if ($(e).val() !== '') {
                totalAmount = totalAmount.plus($(e).val());
            }
        });

        var $totalAmount = $('#allowance-total-amount');
        $totalAmount.val(totalAmount.roundWithCurrency(0, allowanceCurrency).toString());

        if ($totalAmount.hasClass('input-validation-error')) {
            $totalAmount.valid();
        }

        calculateAutoAllowance();
    }

    function convertToAllowanceCurrency(element, allowanceCurrency) {
        var parent = element.closest('.allowance-detail');
        var currency = $('select.allowance-detail-calculate', parent).val();
        var amount = $('input.allowance-detail-calculate', parent).val();
        if (currency !== '' && !isNaN(amount) && amount !== '') {
            amount = new BigNumber(amount);
            var exchangeRates = $('input[data-from-currency="' + currency + '"][data-to-currency="' + allowanceCurrency + '"]').val();

            amount = amount.mul(exchangeRates);
            $('.allowance-convert-amount', parent).val(amount.toString());

            var displayValue = amount.roundWithCurrency(2, allowanceCurrency, false);
            $('.allowance-convert-amount-display', parent).val(displayValue.toStringWithCurrency(allowanceCurrency));
        }
        else {
            $('.allowance-convert-amount', parent).val('');
            $('.allowance-convert-amount-display', parent).val('');
        }
    }

    function calculateAllowances() {
        var allowanceCurrency = $('#Allowance_CurrencyId').val();

        if (allowanceCurrency === '') {
            $('#allowance-total-amount').val('');
            $('.allowance-convert-amount').val('');
            $('.allowance-convert-amount-display').val('');
            $('input.allowance-calculate[readonly]').val('');
            return;
        }

        $('.allowance-detail').each(function (i, e) {
            convertToAllowanceCurrency($(e), allowanceCurrency);
        });
        calculateTotalAllowance(allowanceCurrency);
    }

    function roundingAmount(element, dp, roundingThousand = true) {
        var parent = $(element).parent();
        var $currency = parent.find('select');
        var $amount = parent.find('input.plain-value');
        if ($currency.val() !== '' && $amount.val() !== '' && !isNaN($amount.val())) {
            var number = new BigNumber($amount.val());
            var currency = $currency.val();
            $amount.val(number.roundWithCurrency(dp, currency, roundingThousand).toStringWithCurrency(currency));
        }
    }

    function validatedInvalidRelatedElements(element) {
        var parent = $(element).closest('.group-complex');
        var validateElements = $('input.input-validation-error, select.input-validation-error', parent).not(element);
        if (validateElements.length > 0) {
            validateElements.valid();
        }
    }

    function getAddBankAccountModal(employeeId, selectedBankType) {
        var createBankAccountInPayrollUrl = $("#create-bank-account-in-payroll-url").val();
        $.ajax({
            url: createBankAccountInPayrollUrl + "?employeeId=" + employeeId + "&bankType=" + selectedBankType,
            success: function (data) {
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
            function () {
                if ($(this).hasClass("disabled")) {
                    return;
                }

                var selectedBankType = $(this).parent().find(".bank-type-select").val();
                var employeeId = $("#EmployeeId").val();
                getAddBankAccountModal(employeeId, selectedBankType);
            });
    }

    function initInputMask() {
        $('input.deduction-calculate, input.allowance-detail-calculate, .allowance-convert-amount-display, #duty-meal-per-day').inputmask({
            alias: 'numeric',
            autoGroup: true,
            digits: 2,
            placeholder: '',
            groupSeparator: ',',
            autoUnmask: true,
            removeMaskOnSubmit: true
        });

        $('input.basic-salary-calculate, input.allowance-calculate, #allowance-total-amount, #BasicSalary_Salary_PlainValue').inputmask({
            alias: 'numeric',
            autoGroup: true,
            digits: 0,
            placeholder: '',
            groupSeparator: ',',
            autoUnmask: true,
            removeMaskOnSubmit: true
        });
    }

    function initTooltip() {
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
            arrow: false,
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
    }

    function initSelect2() {
        $('.select2').select2({
            allowClear: true,
            placeholder: ''
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
                url: $('#get-bank-account-url').val(),
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
            minimumResultsForSearch: Infinity
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
    }

    function initFormValidation() {
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
                return value === '' || Number(value) >= 0;
            else
                return true;
        });

        var classNames = ['basic-salary-ebanking', 'basic-salary-directbanking1', 'basic-salary-directbanking2',
            'basic-salary-cash', 'allowance-directbanking1', 'allowance-directbanking2', 'allowance-cash',
            'subsistence', 'home-passage', 'housing', 'mobile-phone', 'hpi', 'duty-meal', 'secondment', 'others',
            'rooming-charges', 'fitness-first'];

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
                    $element.next().find('.select2-selection').addClass('select2-has-error');
            },
            unhighlight: function (element, errorClass, validClass) {
                var $element = $(element);
                $element.removeClass('input-validation-error').addClass(validClass);

                if ($element.hasClass('select2') || $element.hasClass('select2-ajax'))
                    $element.next().find('.select2-selection').removeClass('select2-has-error');
            },
            rules: {
                'BasicSalary.ReasonId': {
                    required: function (element) {
                        return checkCurrencyAndAmountChange(element);
                    }
                },
                'BasicSalary.EffectiveDate': {
                    required: function (element) {
                        return checkCurrencyAndAmountChange(element);
                    }
                },
                'BasicSalary.Salary.PlainValue': {
                    required: function (element) {
                        settingValues = $('.basic-salary-calculate, #BasicSalary_Salary_CurrencyId').map(function (i, element) {
                            if (element.value !== '') {
                                return element.value;
                            }
                        });

                        return settingValues.length > 0;
                    }
                },
                'BasicSalary.Salary.CurrencyId': {
                    required: function (element) {
                        settingValues = $('.basic-salary-calculate, #BasicSalary_Salary_PlainValue').map(function (i, element) {
                            if (element.value !== '') {
                                return element.value;
                            }
                        });

                        return settingValues.length > 0;
                    }
                },
                'Allowance.CurrencyId': {
                    required: function (element) {
                        var allowanceDetailValues = $('.allowance-detail-calculate, .allowance-calculate').map(function (i, element) {
                            if (element.value !== '') {
                                return element.value;
                            }
                        });

                        return allowanceDetailValues.length > 0;
                    }
                },
                'AllowanceTotal': {
                    required: function (element) {
                        var allowanceDetailValues = $('.allowance-detail-calculate, .allowance-calculate').map(function (i, element) {
                            if (element.value !== '') {
                                return element.value;
                            }
                        });

                        return allowanceDetailValues.length > 0;
                    }
                }
            }
        });
    }

    function initEvents() {
        $('.bank-account-select').on('change', function (e) {
            validatedInvalidRelatedElements(e.target);
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

        $('#BasicSalary_Salary_PlainValue, #BasicSalary_Salary_CurrencyId').on('change', function (e) {
            roundingAmount(e.target, 0);
            validatedInvalidRelatedElements(e.target);
            calculateAutoBasicSalary();
        });

        $('.basic-salary-calculate').on('change', function (e) {
            roundingAmount(e.target, 0);
            validatedInvalidRelatedElements(e.target);
            calculateAutoBasicSalary();

            $('#BasicSalary_Salary_PlainValue, #BasicSalary_Salary_CurrencyId').each(function (i, element) {
                var $element = $(element);
                if ($element.hasClass('input-validation-error')) {
                    $element.valid();
                }
            });
        });

        $('.allowance-calculate').on('change', function (e) {
            roundingAmount(e.target, 0);
            validatedInvalidRelatedElements(e.target);
            calculateAutoAllowance();

            $('#allowance-total-amount, #Allowance_CurrencyId').each(function (i, element) {
                var $element = $(element);
                if ($element.hasClass('input-validation-error')) {
                    $element.valid();
                }
            });
        });

        $('.allowance-detail-calculate').on('change', function (e) {
            roundingAmount(e.target, 2, false);
            validatedInvalidRelatedElements(e.target);

            var $totalAmount = $('#allowance-total-amount');
            if ($totalAmount.hasClass('input-validation-error')) {
                $totalAmount.valid();
            }

            var $totalAmountCurrency = $('#Allowance_CurrencyId');
            if ($totalAmountCurrency.hasClass('input-validation-error')) {
                $totalAmountCurrency.valid();
            }

            var allowanceCurrency = $totalAmountCurrency.val();
            if (allowanceCurrency !== '') {
                convertToAllowanceCurrency($(this), allowanceCurrency);
                calculateTotalAllowance(allowanceCurrency);
            }
        });

        $('.deduction-calculate').on('change', function (e) {
            validatedInvalidRelatedElements(e.target);
            roundingAmount(e.target, 2, false);
        });

        $('#Allowance_CurrencyId').on('change', function () {
            $('.allowance-total-currency').val(this.options[this.selectedIndex].text);
            calculateAllowances();
        });

        $('input[name="BasicSalary.WorkingDays"], #duty-meal-per-day').on('change', function (e) {
            var workingDays = $('input[name="BasicSalary.WorkingDays"]:checked').val();
            $('#working-days').val(workingDays);
            var dutyMealPerDay = $('#duty-meal-per-day').val();

            if (dutyMealPerDay !== '') {
                $('#Allowance_DutyMeal_PlainValue').val(workingDays * dutyMealPerDay).change().valid();
            } else {
                $('#Allowance_DutyMeal_PlainValue').val('').change();
            }
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

        $('#btn-save').on('click', function () {
            if ($form.valid()) {
                var effectiveDate = $('#BasicSalary_EffectiveDate').val();
                var basicSalary = $('#BasicSalary_Salary_PlainValue').val();
                var oldSalary = $('#BasicSalary_OldSalary_PlainValue').val();
                var currency = $('#BasicSalary_Salary_CurrencyId').val();
                var oldCurrency = $('#BasicSalary_OldSalary_CurrencyId').val(); 

                if (effectiveDate !== '' && (Number(basicSalary) !== Number(oldSalary) || oldCurrency !== currency)) {
                    $.ajax({
                        type: 'GET',
                        url: commonFunctions.rootURL + 'Employee/CheckEmployeePayrollChangedInMonth',
                        data: {
                            month: effectiveDate,
                            employeeId: $('#EmployeeId').val(),
                        },
                        success: function (response) {
                            if (response) {
                                $('#modal').modal('show');
                            } else {
                                submitForm();
                            }
                        },
                        error: commonFunctions.handleAjaxError
                    });
                } else {
                    submitForm();
                }
            }
        });

        $('#modal #btn-yes').on('click', function () {
            submitForm();
            $('#modal').modal('hide');
        });
    }

    function disableForm() {
        $('#btn-save').off('click');
        $form.find('input[type="text"]').attr('readonly', 'readonly');
        $form.find('select').attr('disabled', 'disabled');
        $form.submit(false);
    }

    function decryptData() {
        $('.encrypted-value').each(function () {
            var $this = $(this);

            if ($this.val() !== '') {
                $($this.data('plain-target')).val(rijndaelService.decryptData($this.val(), rijndaelKey, rijndaelIV));
            } else {
                $($this.data('plain-target')).val('');
            }
        });
    }

    function encryptData() {
        $plainValues.each(function () {
            var $this = $(this);

            if ($this.val() !== '') {
                $($this.data('encrypt-target')).val(rijndaelService.encryptData($this.val(), rijndaelKey, rijndaelIV));
            } else {
                $($this.data('encrypt-target')).val('');
            }
        });
    }

    function submitForm() {
        encryptData();
        $plainValues.attr('disabled', 'disabled');
        $form.submit();
    }

    function init() {
        vndCurrencyId = $('#VNDCurrencyId').val();
        rijndaelIV = $('#RijndaelIV').val();
        rijndaelKey = $('#RijndaelKey').val();
        $plainValues = $('.plain-value');
        $form = $('#payroll-form');

        decryptData();
        calculateAutoBasicSalary();
        calculateAllowances();
        assignAddBankAccountButton();

        initInputMask();
        initTooltip();
        initSelect2();

        $('.allowance-total-currency').val($("#Allowance_CurrencyId option:selected").text());

        if (!$('#CanUpdate').prop('checked')) {
            disableForm();
            return;
        }

        initEvents();
        initFormValidation();

        $('.add-account:not(.disabled)').each(function (index, element) {
            var $element = $(element);
            var bankType = $element.siblings('.bank-type-select').val();
            if (bankType === '3' || bankType === '') {
                $element.addClass('disabled');
            }
        });
      
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
    }

    return {
        init: init,
        onSaveSuccess: function (response, status) {
            if (response.isSuccessful) {
                disableForm();

                var $element = $('.msg-success'),
                    $countDownElement = $element.find('.count-down');
                var delay = 3;
                $element.removeClass('hidden');

                var intervalCountDown = setInterval(function () {
                    if (delay === 0) {
                        clearInterval(intervalCountDown);

                        window.location.href = response.redirectTo;
                    } else {
                        delay--;
                        $countDownElement.html(delay);
                    }
                }, 1000);
            } else {
                if (response.message) {
                    commonFunctions.showErrorMessage($('.msg-error'), response.message);
                }
            }
        },
        onSaveComplete: function () {
            $plainValues.removeAttr('disabled');
        }
    };
}(jQuery, rijndaelService));

$(document).ready(function () {
    employeePayroll.init();
});