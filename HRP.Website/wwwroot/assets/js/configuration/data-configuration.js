var dataConfiguration = (function ($) {
    var $filterFieldForm, $createItemForm;

    function initModule() {
        $filterFieldForm = $('#filterFieldForm');
        $createItemForm = $('#createItemForm');
        registerEventShowListItem();
        registerEventSelectField();
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

    function registerEventSelectField() {
        $('#fieldType').on('change', function () {
            $('#fieldId').val($(this).val());
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

    function initValidationForm(formElementId) {
        var $form = $(formElementId);
        $.validator.unobtrusive.parse(formElementId);

        $form.on('change keyup click submit', function () {
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

    function registerEventClickIconEditItem() {
        $('.js-iconEditItem').on('click', function () {
            var itemId = $(this).data('item-id');
            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + $(this).data('url-update'),
                dataType: 'html',
                data: { "id": itemId },
                success: function (data) {
                    // ref
                    $('#editItemModalBody').html(data);

                    initValidationForm('#editItemForm');

                    $('#editItemModal').modal('show');
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function onCreateNewItemSuccess(response) {
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
    }

    function onUpdateItemSuccess(response) {
        var $editItemForm = $('#editItemForm');
        if (response.isSuccessful) {
            $("#editItemModal .close").trigger('click'); ////ref
            $('.modal-backdrop').remove();
            commonFunctions.showSuccessMessage(response.message);////ref
            $filterFieldForm.trigger('submit'); ////ref
        } else {
            if (response.errors) {
                $editItemForm.validate().showErrors(response.errors); //todo test here
                commonFunctions.showValidationSummary($editItemForm);
            } else {
                alert(response.message);
            }
        }
    }

    function onGetSuccess(response) {
        $('#tableWrapper').html(response);
        initDataTable();
        registerEventQuickSearch();
        initValidationForm('#createItemForm');
        registerEventClickIconEditItem();
    }

    return {
        initModule: initModule,
        onCreateNewItemSuccess: onCreateNewItemSuccess,
        onUpdateItemSuccess: onUpdateItemSuccess
    };
}(jQuery));

$(document).ready(function () {
    dataConfiguration.initModule();
});