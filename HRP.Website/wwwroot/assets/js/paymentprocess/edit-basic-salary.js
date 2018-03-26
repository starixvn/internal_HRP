'use strict';

var editBasicSalary = (function ($) {
    var $form, $btnPreset, $btnRefresh, $btnCancel, $btnSubmit, $btnSubmitAndEditAllowances,
        employeeId, currencySettingId, salaryPerDay, afterAdjustmentAmount,
        maxDigit = 6,
        bankAcountUrl = 'BankAccount/GetEmployeeBankAccounts';

    BigNumber.prototype.roundAmount = function (currencyId) {
        if (currencyId === '11') {
            return this.div(1000).round().mul(1000);
        }

        return this.round(2);
    };

    BigNumber.prototype.toFixedAmount = function (currencyId) {
        if (currencyId === '11')
            return this.toString();

        return this.toFixed(2);
    };

    function getNumber($element) {
        var value = $element.val().trim().replace(',', '');

        if (!isNaN(Number(value))) {
            return $element.val().trim().replace(',', '');
        } else {
            return '';
        }
    }

    function calculateTotalAddition() {
        var hasValue = false,
            totalAmount = new BigNumber(0),
            deduction = Number(getNumber($('.js-deduction-totalNetSalary'))),
            monthlySalary = afterAdjustmentAmount.minus(deduction);
        $('.js-addition-netSalary').each(function () {
            var $this = $(this),
                value = getNumber($this);

            if (value.length > 0 && !isNaN(Number(value))) {
                hasValue = true;
                totalAmount = totalAmount.plus(Number(value));
            }
        });

        if (hasValue) {
            var addition = totalAmount.round(maxDigit);

            $('.js-addition-totalNetSalaryDisplay').val(totalAmount.roundAmount(currencySettingId).toFixedAmount(currencySettingId));
            $('.js-addition-totalNetSalary').val(addition);
            monthlySalary = monthlySalary.plus(addition);
        } else {
            $('.js-addition-totalNetSalaryDisplay').val('');
            $('.js-addition-totalNetSalary').val('');
        }

        monthlySalary = monthlySalary.roundAmount(currencySettingId).toFixedAmount(currencySettingId);
        $('#MonthlySalary_Salary').val(monthlySalary);
        $('.js-totalNetSalary').val(monthlySalary);
        calculateMonthlyBasicSalary();
    }

    function calculateTotalDeduction() {
        var hasValue = false,
            totalAmount = new BigNumber(0),
            addition = Number(getNumber($('.js-addition-totalNetSalary'))),
            monthlySalary = afterAdjustmentAmount.plus(addition);
        $('.js-deduction-netSalary').each(function () {
            var $this = $(this),
                value = getNumber($this);

            if (value.length > 0 && !isNaN(Number(value))) {
                hasValue = true;
                totalAmount = totalAmount.plus(Number(value));
            }
        });

        if (hasValue) {
            var deduction = totalAmount.round(maxDigit);

            $('.js-deduction-totalNetSalaryDisplay').val(totalAmount.roundAmount(currencySettingId).toFixedAmount(currencySettingId));
            $('.js-deduction-totalNetSalary').val(deduction);
            monthlySalary = monthlySalary.minus(deduction);
        } else {
            $('.js-deduction-totalNetSalaryDisplay').val('');
            $('.js-deduction-totalNetSalary').val('');
        }

        monthlySalary = monthlySalary.roundAmount(currencySettingId).toFixedAmount(currencySettingId);
        $('#MonthlySalary_Salary').val(monthlySalary);
        $('.js-totalNetSalary').val(monthlySalary);
        calculateMonthlyBasicSalary();
    }

    function calculateMonthlyBasicSalary() {
        var monthlySalary = Number(getNumber($('#MonthlySalary_Salary'))),
            $autoCalculateOn = $('input[name="MonthlySalary.AutoCalculateOn"]:checked'),
            $parentAutoCalculateOn = $autoCalculateOn.closest('.js-monthlySalary'),
            autoCalculateCurrencyId = $parentAutoCalculateOn.find('.js-selectCurrency').val(),
            currencyId, exchangeRate;
        if (monthlySalary.length === 0 || currencySettingId < 1 || autoCalculateCurrencyId === '') {
            return;
        }

        monthlySalary = new BigNumber(monthlySalary);
        $('input[name="MonthlySalary.AutoCalculateOn"]:not(:checked)').each(function () {
            var $this = $(this),
                $parent = $this.closest('.js-monthlySalary');
            currencyId = $parent.find('.js-selectCurrency').val();

            if (currencyId !== '') {
                var amount = getNumber($parent.find('.js-autoCalculateAmount'));
                if (amount.length > 0 && !isNaN(amount)) {
                    amount = new BigNumber(amount);
                    exchangeRate = Number($('input[data-from-currency="' + currencyId + '"][data-to-currency="' + currencySettingId + '"]').val());
                    monthlySalary = monthlySalary.minus(amount.mul(exchangeRate));
                }
            }
        });

        currencyId = $parentAutoCalculateOn.find('.js-selectCurrency').val();
        exchangeRate = Number($('input[data-from-currency="' + currencySettingId + '"][data-to-currency="' + currencyId + '"]').val());
        monthlySalary = monthlySalary.mul(exchangeRate).roundAmount(currencyId);
        $parentAutoCalculateOn.find('.js-autoCalculateAmount').val(monthlySalary.toFixed(2));
    }

    function initSelect2() {
        $('.js-select2-disabled').select2();

        $('.js-select2-allowclear').select2({
            placeholder: '',
            allowClear: true
        });

        $('.js-select2-bankType').select2({
            placeholder: '',
            allowClear: true,
            minimumResultsForSearch: Infinity
        });

        $('.js-select2-bankAcount').select2({
            ajax: {
                url: commonFunctions.rootURL + bankAcountUrl,
                data: function (params) {
                    var query = {
                        employeeId: employeeId,
                        bankTypeId: $(this).siblings('.js-bankType').val()
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
        });
    }

    function registerInputAdditionEvent() {
        $('.js-addition-unitAmount, .js-addition-noOfDays').on('change', function () {
            var $this = $(this),
                $netSalaryDisplay = $this.siblings('.js-addition-netSalaryDisplay'),
                $netSalary = $this.siblings('.js-addition-netSalary'),
                $relatedEl,
                value = getNumber($this),
                unitAmount, noOfDays, amount;

            if (value.length > 0) {
                amount = new BigNumber(Number(value));

                if ($this.hasClass('js-addition-unitAmount')) {
                    $relatedEl = $this.siblings('.js-addition-noOfDays');
                    noOfDays = Number(getNumber($relatedEl));

                    if (getNumber($relatedEl).length > 0) {
                        amount = amount.mul(noOfDays);

                        $netSalaryDisplay.val(amount.roundAmount(currencySettingId).toFixedAmount(currencySettingId));
                        $netSalary.val(amount.round(maxDigit));
                    } else {
                        $netSalaryDisplay.val(amount.roundAmount(currencySettingId).toFixedAmount(currencySettingId));
                        $netSalary.val(amount.round(maxDigit));
                    }
                } else {
                    $relatedEl = $this.siblings('.js-addition-unitAmount');
                    unitAmount = Number(getNumber($relatedEl));

                    if (getNumber($relatedEl).length > 0) {
                        amount = amount.mul(unitAmount);

                        $netSalaryDisplay.val(amount.roundAmount(currencySettingId).toFixedAmount(currencySettingId));
                        $netSalary.val(amount.round(maxDigit));
                    } else {
                        $netSalaryDisplay.val('');
                        $netSalary.val('');
                    }
                }
            } else {
                if ($this.hasClass('js-addition-unitAmount')) {
                    $netSalaryDisplay.val('');
                    $netSalary.val('');
                } else {
                    $relatedEl = $this.siblings('.js-addition-unitAmount');
                    unitAmount = new BigNumber(Number(getNumber($relatedEl)));

                    if (getNumber($relatedEl).length > 0) {
                        $netSalaryDisplay.val(unitAmount.roundAmount(currencySettingId).toFixedAmount(currencySettingId));
                        $netSalary.val(unitAmount.round(maxDigit));
                    } else {
                        $netSalaryDisplay.val('');
                        $netSalary.val('');
                    }
                }
            }

            calculateTotalAddition();
        });
    }

    function registerInputDeductionEvent() {
        $('.js-unpaidleave-absenteeism, .js-tardiness, .js-deduction-unitAmount').on('change', function () {
            var $this = $(this),
                value = getNumber($this),
                amount;

            if (value.length > 0 && !isNaN(Number(value))) {
                if ($this.hasClass('js-unpaidleave-absenteeism')) {
                    amount = salaryPerDay.mul(Number(value));
                } else if ($this.hasClass('js-tardiness')) {
                    amount = salaryPerDay.div(8 * 60).mul(Number(value));
                } else {
                    amount = new BigNumber(Number(value));
                }

                $this.siblings('.js-deduction-netSalaryDisplay').val(amount.roundAmount(currencySettingId));
                $this.siblings('.js-deduction-netSalary').val(amount.round(maxDigit));
            } else {
                $this.siblings('.js-deduction-netSalaryDisplay').val('');
                $this.siblings('.js-deduction-netSalary').val('');
            }

            calculateTotalDeduction();
        });
    }

    function registerInputNumberEvent() {
        $('.js-numberOnly').inputmask({
            alias: 'numeric',
            autoGroup: true,
            digits: 2,
            placeholder: '',
            allowMinus: false,
            groupSeparator: ','
        });
    }

    function registerRefreshPageEvent() {
        $btnRefresh.on('click', function () {
            location.reload();
        });
    }

    function customAnnotation() {
        $.validator.addMethod('requiredifrelatedfieldhasvalue', function (value, element, params) {
            var $element = $(element),
                $relatedElement = $element.siblings('.js-addition-noOfDays');
            if ($relatedElement.length > 0 && getNumber($relatedElement) !== '' && getNumber($element) === '') {
                return false;
            }

            return true;
        });

        $.validator.addMethod('requirednoneorall', function (value, element, params) {
            var $element = $(element),
                dataGroup = $element.data('group'),
                $relatedElement = $('[data-group="' + dataGroup + '"]').not($element);

            var relatedElementValues = $relatedElement.map(function (i, element) {
                if (element.value !== '') {
                    return element.value;
                }
            });

            return relatedElementValues.length === 0 || element.value !== '';
        });

        $.validator.unobtrusive.adapters.add('requiredifrelatedfieldhasvalue', [], function (options) {
            options.rules['requiredifrelatedfieldhasvalue'] = [];
            options.messages['requiredifrelatedfieldhasvalue'] = options.message;
        });

        $.validator.unobtrusive.adapters.add('requirednoneorall', [], function (options) {
            options.rules['requirednoneorall'] = [];
            options.messages['requirednoneorall'] = options.message;
        });
    }

    function reparseform($form) {
        customAnnotation();

        $form.removeData('validator').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse($form);

        var $validator = $form.validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);

            if ($element.hasClass('js-select2-validation')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(errorClass).addClass('control');
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('js-select2-validation')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(validClass);
            }
        };

        $form.find('.js-select2-validation').on('change', function () {
            var $this = $(this);
            var $select2 = $this.next().find('.select2-selection');

            if ($this.val() && $select2.hasClass('input-validation-error')) {
                $select2.removeClass('input-validation-error').removeClass('control');
                $this.valid();
            }
        });
    }

    function displaySuccessMessageAndRedirectToEditAllowances(redirectTo) {
        var $element = $form.find('.js-submitSuccessAndEditAllowances'),
            $countDownElement = $element.find('.count-down');
        var delay = 5;
        $element.removeClass('hidden');

        var intervalCountDown = setInterval(function () {
            if (delay === 0) {
                clearInterval(intervalCountDown);

                window.location.href = redirectTo;
            } else {
                delay--;
                $countDownElement.html(delay);
            }
        }, 1000);
    }

    function registerEvents() {
        registerRefreshPageEvent();
        registerInputNumberEvent();
        registerInputAdditionEvent();
        registerInputDeductionEvent();

        $('input[name="MonthlySalary.AutoCalculateOn"]').on('change', function () {
            var $this = $(this),
                $parent = $this.closest('.js-monthlySalary');

            if (this.checked) {
                $('.js-autoCalculateAmount[readonly]').removeAttr('readonly');
                $parent.find('.js-numberOnly').attr('readonly', 'readonly');
            }
        }).change();

        $('.js-selectCurrency, .js-autoCalculateAmount').on('change', function () {
            calculateMonthlyBasicSalary();
        });

        $btnSubmit.on('click', function () {
            initSelect2();

            if ($form.valid()) {
                $form.submit();
            }
        });

        $btnSubmitAndEditAllowances.on('click', function () {
            initSelect2();

            if ($form.valid()) {
                $('#IsRedirectToEditAllowance').prop('checked', true);
                $form.submit();
            }
        });
    }

    function initModule() {
        $btnPreset = $('.js-btnPreset');
        $btnRefresh = $('.js-btnRefresh');
        $btnCancel = $('.js-btnCancel');
        $btnSubmit = $('.js-btnSubmit');
        $btnSubmitAndEditAllowances = $('.js-btnSubmitAndEditAllowances');
        $form = $('#edit-basic-salary-form');
        employeeId = parseInt($('#EmployeeId').val(), 10);
        currencySettingId = parseInt($('#MonthlySalary_CurrencyId').val(), 10);
        salaryPerDay = new BigNumber(Number(getNumber($('#MonthlyBasicSalary_SalaryPerDay'))));
        afterAdjustmentAmount = new BigNumber(Number(getNumber($('#MonthlyBasicSalary_AfterAdjustmentAmount'))));

        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
        commonFunctions.registerEventGoBack($btnCancel);
        initSelect2();
        registerEvents();
        reparseform($form);

        if (!$('#IsHost').prop('checked')) {
            $form.find('input[type="text"]').attr('readonly', 'readonly');
            $form.find('select').attr('disabled', 'disabled');
            $form.submit(false);
        }
    }

    return {
        initModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                if ($('#IsRedirectToEditAllowance').prop('checked')) {
                    displaySuccessMessageAndRedirectToEditAllowances(response.redirectTo);
                } else {
                    commonFunctions.showSuccessMessage($form.find('.js-submitSuccess'));
                }

                $('#IsRedirectToEditAllowance').prop('checked', false);
            } else {
                if (response.errors) {
                    $form.validate().showErrors(response.errors);
                } else {
                    commonFunctions.showErrorMessage($form.find('.msg-error'), response.message);
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    editBasicSalary.initModule();
});