var dataConfiguration = (function ($) {
    var $filterFieldForm;

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
            var $fieldType = $('#fieldType');
            var $selectedField = $('#selectedField');
            var fieldId = $fieldType.val();
            var getAllUrl = $fieldType.find(':selected').data('url-get-all');
            $selectedField.val(fieldId);

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

    function registerEventClickCreateNewItem() {
        $('.js-createNewItem').on('click', function () {
            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + $(this).data('url-create'),
                dataType: 'html',
                success: function (data) {
                    // ref
                    $('#createItemModalContent').html(data);

                    initValidationForm('#createItemForm');

                    $('#newItemModal').modal('show');
                },
                error: commonFunctions.handleAjaxError
            });
        });
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

    function reloadListItem() {
        var $fieldType = $('#fieldType');
        var $selectedField = $('#selectedField');

        $fieldType.val($selectedField.val());
        $filterFieldForm.trigger('submit'); ////ref
    }

    function onCreateNewItemSuccess(response) {
        var  $createItemForm = $('#createItemForm');
        if (response.isSuccessful) {
            $("#newItemModal .close").trigger('click'); ////ref
            $('.modal-backdrop').remove();
            commonFunctions.showSuccessMessage(response.message);////ref
            reloadListItem();
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
            reloadListItem();
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
        //initValidationForm('#createItemForm');
        registerEventClickCreateNewItem();
        registerEventClickIconEditItem();
    }

    function initModule() {
        $filterFieldForm = $('#filterFieldForm');
        registerEventShowListItem();
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