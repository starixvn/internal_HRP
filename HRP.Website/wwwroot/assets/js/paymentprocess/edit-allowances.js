var editAllowances = (function ($) {
    function setValidationValues(options, ruleName, value) {
        options.rules[ruleName] = value;
        if (options.message) {
            options.messages[ruleName] = options.message;
        }
    }

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
        })

        var $editAllowanceForm = $("#edit-allowances-form");
        $editAllowanceForm.removeData("validator").removeData("unobtrusiveValidation");

        $.validator.addMethod('requiredif', function (value, element, params) {
            var $element = $(element);
            var dataGroup = $element.data('group');
            var $relatedElement = $('input[data-group="' + 'dataGroup"]');
            if ($relatedElement.val() !== '' && $element.val() === '') {
                return false;
            }

            return true;
        });

        $.validator.unobtrusive.adapters.add('requiredif', [], function (options) {
            options.rules['requiredif'] = options.params['year'];
            options.messages['requiredif'] = options.message;.l
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