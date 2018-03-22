var editAllowances = (function ($) {
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
                url: $(this).data('url'),
                data: function (params) {
                    var query = {
                        //employeeId: $('#EmployeeId').val(),
                        //bankTypeId: $(this).siblings('.bank-type-select').val()
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

        $('.addtion, .deduction').on('change', function (e) {
            var $this = $(this);
            var dataGroup = $this.data('group');
            var $relatedElements = $('input[data-group="' + dataGroup + '"]');

            var $unitAmount = $('.unit-amount', $relatedElements);
            var $currency = $('.currency', $relatedElements);
            var $noOfDays = $('.no-of-days', $relatedElements);
            var $totalAmount = $('.total-amount', $relatedElements);
            var $netAllowance = $('.net-allowance', $relatedElements);

            if ($unitAmount.val() !== '') {
                var unitAmount = new BigNumber($unitAmount.val());
                if ($noOfDays.val() !== '') {
                    unitAmount = unitAmount.mul($noOfDays.val());
                    $totalAmount.val(unitAmount.toString());
                } else {
                    $totalAmount.val(unitAmount.toString());
                }


            } else {
                $totalAmount.val('');
                $netAllowance.val('');
            }
        });

        var $editAllowanceForm = $("#edit-allowances-form");
        $editAllowanceForm.removeData("validator").removeData("unobtrusiveValidation");

        $.validator.addMethod('requiredifrelatedfieldhasvalue', function (value, element, params) {
            var $element = $(element);
            var dataGroup = $element.data('group');
            var $relatedElement = $('input[data-group="' + dataGroup + '"]');
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
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    editAllowances.initModule();
});