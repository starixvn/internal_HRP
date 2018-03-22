'use strict';

var editBasicSalary = (function ($) {
    var $btnPreset, $btnRefresh, $btnCancel, $form,
        currencySettingId, salaryPerDay, afterAdjustmentAmount;

    BigNumber.prototype.roundAmountWithCurrency = function (currencyId) {
        if (currencyId === '11') {
            return this.div(1000).round().mul(1000);
        }

        return this.round(2);
    };

    function calculateTotalAddition() {
        var hasValue = false,
            totalAmount = new BigNumber(0);
        $('.js-addition-netSalaryDisplay').each(function () {
            var $this = $(this),
                value = $this.val().trim();

            if (value.length > 0 && !isNaN(Number(value))) {
                hasValue = true;
                totalAmount = totalAmount.plus(Number(value));
            }
        });

        if (hasValue) {
            $('.js-addition-totalNetSalaryDisplay').val(totalAmount.roundAmountWithCurrency(currencySettingId));
            $('.js-addition-totalNetSalary').val(totalAmount);
        }
    }

    function calculateTotalDeduction() {
        var hasValue = false,
            totalAmount = new BigNumber(0);
        $('.js-deduction-netSalaryDisplay').each(function () {
            var $this = $(this),
                value = $this.val().trim();

            if (value.length > 0 && !isNaN(Number(value))) {
                hasValue = true;
                totalAmount = totalAmount.plus(Number(value));
            }
        });

        if (hasValue) {
            $('.js-deduction-totalNetSalaryDisplay').val(totalAmount.roundAmountWithCurrency(currencySettingId));
            $('.js-deduction-totalNetSalary').val(totalAmount);
        }
    }

    function initSelect2() {
        $('.js-select2-disabled').select2();

        $('.js-select2-allowclear').select2({
            placeholder: '',
            allowClear: true
        });

        $('.js-select2-allowclear-hidesearchbox').select2({
            placeholder: '',
            allowClear: true,
            minimumResultsForSearch: Infinity
        });
    }

    function registerInputEventForAddition() {
        $('.js-addition-unitAmount, .js-addition-noOfDays').on('change', function () {
            var $this = $(this),
                $relatedEl,
                value = $this.val().trim(),
                unitAmount,
                noOfDays,
                amount;

            if (value.length > 0) {
                if ($this.hasClass('js-addition-unitAmount')) {
                    $relatedEl = $this.siblings('.js-addition-noOfDays');
                    noOfDays = Number($relatedEl.val().trim());

                    if ($relatedEl.val().trim().length === 0) {
                        amount = new BigNumber(Number(value));
                        $this.siblings('.js-addition-netSalaryDisplay').val(amount.roundAmountWithCurrency(currencySettingId));
                        $this.siblings('.js-addition-netSalary').val(amount);

                        calculateTotalAddition();
                    } else if (!isNaN(noOfDays)) {
                        amount = new BigNumber(Number(value));
                        amount = amount.mul(noOfDays);
                        $this.siblings('.js-addition-netSalaryDisplay').val(amount.roundAmountWithCurrency(currencySettingId));

                        calculateTotalAddition();
                    }
                } else {
                    unitAmount = Number($this.siblings('.js-addition-unitAmount').val().trim());
                    if (!isNaN(unitAmount)) {
                        amount = unitAmount * Number(value);
                        $this.siblings('.js-addition-netSalaryDisplay').val(amount);

                        calculateTotalAddition();
                    }
                }
            }
        });
    }

    function registerInputEventForDeduction() {
        $('.js-unpaidleave-absenteeism, .js-tardiness, .js-deduction-unitAmount').on('change', function () {
            var $this = $(this),
                value = $this.val().trim(),
                amount;

            if (value.length > 0 && !isNaN(Number(value))) {
                if ($this.hasClass('js-unpaidleave-absenteeism')) {
                    amount = salaryPerDay.mul(Number(value));
                } else if ($this.hasClass('js-tardiness')) {
                    amount = salaryPerDay.div(8 * 60).mul(Number(value));
                } else {
                    amount = Number(value);
                }

                $this.siblings('.js-deduction-netSalaryDisplay').val(amount);
                calculateTotalDeduction();
            }
        });
    }

    function registerRefreshPageEvent() {
        $btnRefresh.on('click', function () {
            location.reload();
        });
    }

    function initModule() {
        $btnPreset = $('.js-btnPreset');
        $btnRefresh = $('.js-btnRefresh');
        $btnCancel = $('.js-btnCancel');
        $form = $('#edit-basic-salary-form');
        currencySettingId = $('#MonthlyBasicSalary_CurrencyId').val();
        salaryPerDay = new BigNumber(Number($('#SalaryPerDay').val()));
        afterAdjustmentAmount = new BigNumber(Number($('#MonthlyBasicSalary_AfterAdjustmentAmount').val()));

        registerRefreshPageEvent();
        registerInputEventForAddition();
        registerInputEventForDeduction();

        initSelect2();

        $('.js-numberOnly').on('keyup', function (e) {
            var val = $(this).val(),
                regex = /^\d*\.?([0-9]?[0-9])?$/g,
                regex1 = /^\d*\.?([0-9]?[0-9])?/g;

            if (!regex.test(val)) {
                val = regex1.exec(val);

                if (val) {
                    $(this).val(val[0]);
                } else {
                    $(this).val('');
                }
            }
        });
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    editBasicSalary.initModule();
});