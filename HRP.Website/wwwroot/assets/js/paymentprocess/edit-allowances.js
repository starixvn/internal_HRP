var editAllowances = (function ($, commonFunctions) {
    var totalAmountCurrencyId;
    var $editAllowanceForm;
    var workingDays;

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
        $('.addition.net-allowance').each(function (i, element) {
            totalAddition = totalAddition.plus(Number(element.value));
        });

        $("#total-addition").val(totalAddition.toString());
        $("#total-addition-display").val(totalAddition.roundWithCurrency(2, totalAmountCurrencyId).toStringWithCurrency(totalAmountCurrencyId));
    };

    function calculateTotalDeduction() {
        var totalDeduction = new BigNumber(0);
        $('.deduction.net-allowance').each(function (i, element) {
            totalDeduction = totalDeduction.plus(Number(element.value));
        });

        $("#total-deduction").val(totalDeduction.toString());
        $("#total-deduction-display").val(totalDeduction.roundWithCurrency(2, totalAmountCurrencyId).toStringWithCurrency(totalAmountCurrencyId));
    };

    function calculateTotalAmount() {
        var totalAmount = new BigNumber(Number($('#BasicAllowance_AfterAdjustmentAmount').val()));

        totalAmount = totalAmount.plus(Number($("#total-addition").val()));
        totalAmount = totalAmount.minus(Number($("#total-deduction").val()));

        var fitnessFirstCurrencyId = $('#BasicAllowance_FitnessFirstCurrencyId').val();
        if (fitnessFirstCurrencyId !== '') {
            var fitnessFirstAmount = new BigNumber(Number($('#BasicAllowance_FitnessFirstAmount').val()));
            var exchangeRate = Number($('input[data-from-currency="' + fitnessFirstCurrencyId + '"][data-to-currency="' + totalAmountCurrencyId + '"]').val());
            totalAmount = totalAmount.minus(fitnessFirstAmount.mul(exchangeRate));
        }

        var roomingChargesCurrencyId = $('#BasicAllowance_RoomingChargesCurrencyId').val();
        if (roomingChargesCurrencyId !== '') {
            var roomingChargesAmount = new BigNumber(Number($('#BasicAllowance_RoomingChargesAmount').val()));
            var exchangeRate = Number($('input[data-from-currency="' + roomingChargesCurrencyId + '"][data-to-currency="' + totalAmountCurrencyId + '"]').val());
            totalAmount = totalAmount.minus(roomingChargesAmount.mul(exchangeRate));
        }

        var totalAmountDisplay = totalAmount.roundWithCurrency(2, totalAmountCurrencyId).toStringWithCurrency(totalAmountCurrencyId);
        $('#total-amount-display').val(totalAmountDisplay);
        $('#Allowance_TotalAmount').val(totalAmountDisplay);
    }

    function calculateTotalNetAmount() {
        var totalNetAmount = new BigNumber(Number($('#Allowance_TotalAmount').val()));
        var dstCurrencyId = $('#Allowance_DSTNetSalaryCurrencyId').val();
        if (dstCurrencyId !== '') {
            var dstAmount = new BigNumber(Number($('#Allowance_DSTNetSalaryAmount').val()));
            var exchangeRate = Number($('input[data-from-currency="' + dstCurrencyId + '"][data-to-currency="' + totalAmountCurrencyId + '"]').val());
            totalNetAmount = totalNetAmount.minus(dstAmount.mul(exchangeRate));
        }

        $('#Allowance_TotalNetAmount').val(totalNetAmount.roundWithCurrency(2, totalAmountCurrencyId).toStringWithCurrency(totalAmountCurrencyId));
    }

    function calculateAutoAllowance() {
        var totalAmount = new BigNumber(Number($('#Allowance_TotalAmount').val()));

        if (totalAmountCurrencyId === '')
            return;

        var $selectedAutoCalculate = $('input[name="Allowance.AutoCalculateOn"]:checked');
        var group = $selectedAutoCalculate.data('calculate-for');
        var autoCalculateCurrencyId = $('select.allowance-calculate[data-calculate-for="' + group + '"]').val();
        if (autoCalculateCurrencyId === '')
            return;

        $('input[name="Allowance.AutoCalculateOn"]:not(:checked)').each(function (i, e) {
            var group = $(e).data('calculate-for');
            var currency = $('select.allowance-calculate[data-calculate-for="' + group + '"]').val();
            if (currency !== '') {
                var $element = $('input.allowance-calculate[type="text"][data-calculate-for="' + group + '"]');
                if ($element.val() !== '' && !isNaN($element.val())) {
                    var amount = new BigNumber(Number($element.val()));
                    var exchangeRates = Number($('input[data-from-currency="' + currency + '"][data-to-currency="' + totalAmountCurrencyId + '"]').val());
                    totalAmount = totalAmount.minus(amount.mul(exchangeRates));
                }
            }
        });

        totalAmount = totalAmount.mul($('input[data-from-currency="' + totalAmountCurrencyId + '"][data-to-currency="' + autoCalculateCurrencyId + '"]').val()).roundWithCurrency(2, autoCalculateCurrencyId);

        var $autoCalculatedElement = $('input.allowance-calculate[type="text"][data-calculate-for="' + group + '"]');
        $autoCalculatedElement.val(totalAmount.toStringWithCurrency(autoCalculateCurrencyId));
        $autoCalculatedElement.valid();
    }

    function initInputMask() {
        $('.unit-amount, .no-of-days, .total-amount-display, .net-allowance-display, #total-addition-display, #total-deduction-display').inputmask({
            alias: 'numeric',
            autoGroup: true,
            digits: 2,
            placeholder: '',
            allowMinus: false,
            groupSeparator: ',',
            autoUnmask: true,
            removeMaskOnSubmit: true,
        })

        $('.number').inputmask({
            alias: 'numeric',
            autoGroup: true,
            digits: 2,
            placeholder: '',
            groupSeparator: ',',
            autoUnmask: true,
            removeMaskOnSubmit: true,
        })
    }

    function initSelect2() {
        $('.select2:disabled').select2();
        $('.select2:enabled').select2({
            allowClear: true,
            placeholder: '',
            minimumResultsForSearch: Infinity
        }).on('change', function () {
            $(this).valid();
        });

        $('.select2-ajax').select2({
            ajax: {
                url: $('#get-bank-acount-url').val(),
                data: function (params) {
                    var query = {
                        employeeId: $('#EmployeeInfo_EmployeeId').val(),
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
        }).on('change', function () {
            $(this).valid();
        });
    }

    function parseForm() {
        $.validator.addMethod('requiredifrelatedfieldhasvalue', function (value, element, params) {
            var $element = $(element);
            var dataGroup = $element.data('group');
            var $relatedElement = $('.no-of-days[data-group="' + dataGroup + '"]');
            if ($relatedElement.val() !== '' && $element.val() === '') {
                return false;
            }

            return true;
        });

        $.validator.addMethod('requirednoneorall', function (value, element, params) {
            var $element = $(element);
            var dataGroup = $element.data('group');
            var $relatedElement = $('[data-group="' + dataGroup + '"]').not($element);
            var relatedElementValues = $relatedElement.map(function (i, element) {
                if (element.value !== '') {
                    return element.value
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

        $editAllowanceForm.removeData("validator").removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse($editAllowanceForm);
        commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));

        var $validator = $editAllowanceForm.validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);

            if ($element.hasClass('select2') || $element.hasClass('select2-ajax')) {
                $element.next().find('.select2-selection').addClass('select2-has-error');
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('select2') || $element.hasClass('select2-ajax')) {
                $element.next().find('.select2-selection').removeClass('select2-has-error');
            }
        };
    }

    function initEvents() {
        $('.bank-type-select').on('change', function (e) {
            $(this).siblings('.bank-account-select').val('').trigger('change.select2');
        });

        $('input[name="Allowance.AutoCalculateOn"]').on('change', function () {
            if (this.checked) {
                var group = $(this).data('calculate-for');
                $('input.allowance-calculate[readonly]').removeAttr('readonly');
                $('input.allowance-calculate[data-calculate-for="' + group + '"]').attr('readonly', 'readonly');
            }
        }).change();

        $('.allowance-calculate').on('change', function (e) {
            calculateAutoAllowance();
        });

        $('.no-of-days').on('change', function (e) {
            var $this = $(this);
            var dataGroup = $this.data('group');
            var $unitAmount = $('.unit-amount[data-group="' + dataGroup + '"]');
            if ($this.val() !== '') {
                var currencyId = $('.currency[data-group="' + dataGroup + '"]').val();

                var amount = new BigNumber(Number($('.basic-allowance-amount.' + $this.data('allowance-type')).val()));
                $unitAmount.val(amount.div(workingDays).roundWithCurrency(2, currencyId).toStringWithCurrency(currencyId));
            } else {
                $unitAmount.val('');
            }
        });

        $('.addition:enabled, .deduction:enabled').on('change', function (e) {
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

                if (currencyId !== '' && totalAmountCurrencyId !== '') {
                    var exchangeRates = Number($('input[data-from-currency="' + currencyId + '"][data-to-currency="' + totalAmountCurrencyId + '"]').val());
                    var netAllowance = totalAmount.mul(exchangeRates);
                    $netAllowance.val(netAllowance.toString());
                    $netAllowanceDisplay.val(netAllowance.roundWithCurrency(2, totalAmountCurrencyId).toStringWithCurrency(totalAmountCurrencyId));
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

            if ($this.hasClass('addition')) {
                calculateTotalAddition();
            } else {
                calculateTotalDeduction();
            }

            calculateTotalAmount();
            calculateAutoAllowance();
            calculateTotalNetAmount();
        });

        $('#Allowance_DSTNetSalaryAmount').on('change', calculateTotalNetAmount);
    }

    function initModule() {
        $editAllowanceForm = $('#edit-allowances-form');
        totalAmountCurrencyId = $('#Allowance_CurrencyId').val();
        workingDays = $('#EmployeeInfo_WorkingDays').val();
        calculateTotalAddition();
        calculateTotalDeduction();

        initSelect2();
        initInputMask();

        $('#btn-refresh').on('click', function () {
            window.location.reload();
        });

        if ($('#CanEdit').val() === 'False') {
            $('input:enabled, select:enabled', $editAllowanceForm).attr('disabled', 'disabled');
            return;
        }

        initEvents();
        parseForm();
    }

    return {
        initModule: initModule,
        onPresetBegin: function () {
            $('#btn-yes').button('loading');
        },
        onPresetSuccess: function (response, status) {
            if (response.isSuccessful) {
                window.location.reload();
            } else {
                commonFunctions.showErrorMessage($('.msg-error'), response.message);
            }

            $('#modal').modal('hide');
        },
        onPresetComplete: function () {
            $('#btn-yes').button('reset');
        }
    };
}(jQuery, commonFunctions));

$(document).ready(function () {
    editAllowances.initModule();
});