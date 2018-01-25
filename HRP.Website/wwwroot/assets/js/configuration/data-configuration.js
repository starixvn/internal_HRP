var dataConfiguration = (function ($) {
    var $filterFieldForm, $createItemForm;

    function initModule() {
        $filterFieldForm = $('#filterFieldForm');
        registerEventShowListItem();
    }

    function initDataTable() {
        $('#tableItems').dataTable({
            'ordering': false,
            'lengthMenu': [[20, 50, 100], [20, 50, 100]],
            'dom': 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            'info': false,
            'language': {
                'lengthMenu': 'Page size _MENU_',
                'search': '',
                'paginate': {
                    'first': 'First',
                    'last': 'Last',
                    'next': '<i class="icon-next"></i>',
                    'previous': '<i class="icon-back"></i>'
                }
            }
        });
    }

    function registerEventQuickSearch() {
        $('#quickSearch').on('keyup', function () {
            $('#tableItems').DataTable().search($(this).val()).draw();
        });
    }

    function registerEventShowListItem() {
        $filterFieldForm.on('submit', function (e) {
            e.preventDefault();

            var fieldId = $('#fieldId').val();
            var getAllUrl = $('.field-' + fieldId).data('url-get-all');

            $.ajax({
                type: 'GET',
                dataType: 'html',
                url: commonFunctions.rootURL + getAllUrl,
                success: onGetSuccess,
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventSubmitFormCreateNewItem() {
        $createItemForm.on('submit', function (e) {
            if ($createItemForm.valid()) {
                e.preventDefault();
                var formData = $(this).serialize();
                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + $(this).attr('action'),
                    dataType: 'json',
                    data: formData,
                    success: function (response) {
                        if (response.isSuccessful) {                           
                            $("#newItem .close").trigger('click'); ////ref
                            $('.modal-backdrop').remove();
                            commonFunctions.showSuccessMessage(response.message);////ref
                            $filterFieldForm.trigger('submit'); ////ref
                        } else {
                            if (response.errors) {
                                $createItemForm.validate().showErrors(response.errors); //todo test here
                                commonFunctions.showValidationSummary($createItemForm);
                            } else {
                                commonFunctions.showErrorMessage(response.message);
                            }
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($createItemForm);
            }
        });
    }

    function initValidationCreateItemForm() {
        //
        $.validator.unobtrusive.parse('#createItemForm');
        var formSetting = $createItemForm.data("validator").settings;
        formSetting.onkeyup = false;
        //
        var fieldItemName = $('#createItemForm .js-itemName');
        fieldItemName.rules().remote.complete = function (data) {
            if ($.parseJSON(data.responseText) !== "true") {
                commonFunctions.showValidationSummary($createItemForm);
            } else {
                handleDisplayValidationSummary($createItemForm);
            }
        };
        //
        $createItemForm.on('change keyup click', function () {
            handleDisplayValidationSummary($(this));
        });
    }

    function handleDisplayValidationSummary($form) {
        var errorTotal = $form.find('.list-message-error .field-validation-error').length;
        if (errorTotal === 0) {
            commonFunctions.hideValidationSummary($form);
        } else {
            commonFunctions.showValidationSummary($form);
        }
    }

    function onGetSuccess(response) {
        $('#tableWrapper').html(response);
        $createItemForm = $('#createItemForm');
        initDataTable();
        registerEventQuickSearch();
        initValidationCreateItemForm();
        registerEventSubmitFormCreateNewItem();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    dataConfiguration.initModule();
});