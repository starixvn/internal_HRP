var dataConfiguration = (function ($) {
    var domElement = {
        FilterFieldForm: '#filterFieldForm',
        CompanyId: '.js-companyId',
        DivisionId: '.js-divisionId',
        DepartmentId: '.js-departmentId',
        CostCenterId: '.js-costCenterId',
        SectionId: '.js-sectionId'
    };
    var htmlDefaultOption = '<option value=""></option>';

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
        $(domElement.FilterFieldForm).on('submit', function (e) {
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

                    registerEventChangeCompanyBVI();
                    registerEventChangeDivision();
                    registerEventChangeDepartment();
                    registerEventChangeCostCenter();

                    var createFormName = '#createItemForm';
                    initValidationForm(createFormName);

                    $('#newItemModal').modal('show');
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeCompanyBVI() {
        $(domElement.CompanyId).on('change', function () {
            //reset division, department, cost center....
            var $divisionId = $(domElement.DivisionId);
            $divisionId.html(htmlDefaultOption);
            $(domElement.DepartmentId).html(htmlDefaultOption);
            $(domElement.CostCenterId).html(htmlDefaultOption);
            $(domElement.SectionId).html(htmlDefaultOption);

            var companyId = $(this).val();
            if (companyId !== "" && $divisionId.length > 0) {
                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'DataConfiguration/GetDivisionSelectListByCompanyId',
                    dataType: 'html',
                    data: { companyId: companyId },
                    success: function (data) {
                        $divisionId.html(data);
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEventChangeDivision() {
        $(domElement.DivisionId).on('change', function () {
            var $departmentId = $(domElement.DepartmentId);
            $departmentId.html(htmlDefaultOption);
            $(domElement.CostCenterId).html(htmlDefaultOption);
            $(domElement.SectionId).html(htmlDefaultOption);

            var divisionId = $(this).val();
            if (divisionId !== "" && $departmentId.length > 0) {
                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'DataConfiguration/GetDepartmentSelectListByDivisionId',
                    dataType: 'html',
                    data: { divisionId: divisionId },
                    success: function (data) {
                        $departmentId.html(data);
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEventChangeDepartment() {
        $(domElement.DepartmentId).on('change', function () {
            var $costCenterId = $(domElement.CostCenterId);
            $costCenterId.html(htmlDefaultOption);
            $(domElement.SectionId).html(htmlDefaultOption);

            var departmentId = $(this).val();
            if (departmentId !== "" && $costCenterId.length > 0) {
                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'DataConfiguration/GetCostCenterSelectListByDepartmentId',
                    dataType: 'html',
                    data: { departmentId: departmentId },
                    success: function (data) {
                        $costCenterId.html(data);
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEventChangeCostCenter() {
        $(domElement.CostCenterId).on('change', function () {
            var $sectionId = $(domElement.SectionId);
            $sectionId.html(htmlDefaultOption);

            var costCenterId = $(this).val();
            if (costCenterId !== "" && $sectionId.length > 0) {
                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'DataConfiguration/GetSectionSelectListByCostCenterId',
                    dataType: 'html',
                    data: { costCenterId: costCenterId },
                    success: function (data) {
                        $sectionId.html(data);
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
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
                    $('#editItemModalContent').html(data);

                    initValidationForm('#editItemForm');

                    $('#editItemModal').modal('show');
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function reloadTableListItem() {
        var $fieldType = $('#fieldType');
        var $selectedField = $('#selectedField');

        $fieldType.val($selectedField.val());
        $(domElement.FilterFieldForm).trigger('submit'); ////ref
    }

    function onCreateNewItemSuccess(response) {
        var $createItemForm = $('#createItemForm');
        if (response.isSuccessful) {
            $("#newItemModal .close").trigger('click'); ////ref
            $('.modal-backdrop').remove();
            commonFunctions.showSuccessMessage(response.message);////ref
            reloadTableListItem();
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
            reloadTableListItem();
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
        registerEventClickCreateNewItem();
        registerEventClickIconEditItem();
    }

    function initModule() {
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