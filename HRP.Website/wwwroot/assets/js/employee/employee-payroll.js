var employeePayroll = (function ($) {
    function checkRelatedElementsHaveValue(className, element) {
        return $(className).not(element).serializeArray().filter(function (item) {
            return item.value != '';
        }).length > 0;
    };

    function init() {
        $('form').validate({
            debug: true,
            submitHandler: function (form) {
                form.submit();
            },
            wrapper: 'li',
            errorContainer: '#validation-summary',
            errorLabelContainer: '#validation-summary ul',
            highlight: function (element, errorClass) {
                debugger;
                $(element).addClass('input-validation-error');
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass('input-validation-error').addClass(validClass);
            },
            rules: {
                'BasicSalary.Salary': {
                    required: true
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
                    }
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