var dataConfiguration = (function ($) {
    var domElement = {
        FieldId: '#FieldId',
        TableItems: '#tableItems',
        FilterFieldForm: '#filterFieldForm',
        SelectedField: '#selectedField',
        CompanyId: '.js-companyId',
        DivisionId: '.js-divisionId',
        DepartmentId: '.js-departmentId',
        CostCenterId: '.js-costCenterId',
        SectionId: '.js-sectionId',
        JobCategoryId: '.js-jobCateogryId',
        GradeLevelId: '.js-gradeLevelId',
        CountryId: '.js-countryId',
        CityId: '.js-cityId',
        CreateItemModal: '#createItemModal',
        CreateItemModalContent: '#createItemModalContent'
    };
    var htmlDefaultOption = '<option value=""></option>';

    function initDataTable() {
        $(domElement.TableItems).dataTable({
            'ordering': false,
            'lengthMenu': [commonFunctions.pagerRanges, commonFunctions.pagerRanges],
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

        var $table = $(domElement.TableItems);
        $table.on('draw.dt', function () {
            registerEventClickIconEditItem();
        });
    }

    function registerEventQuickSearch() {
        $('#quickSearch').on('keyup', function () {
            $(domElement.TableItems).DataTable().search($(this).val()).draw();
        });
    }

    function registerEventShowListItem() {
        $(domElement.FilterFieldForm).on('submit', function (e) {
            e.preventDefault();

            var $fieldId = $(domElement.FieldId);
            var $selectedField = $(domElement.SelectedField);
            var fieldId = $fieldId.val();
            var getAllUrl = $fieldId.find(':selected').data('url-get-all');
            $selectedField.val(fieldId);

            //Clear html create item modal
            $(domElement.CreateItemModalContent).empty();

            $.ajax({
                type: 'GET',
                dataType: 'html',
                data: { "fieldId": fieldId },
                url: commonFunctions.rootURL + getAllUrl,
                success: onGetSuccess,
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function initValidationForm($form) {
        $.validator.unobtrusive.parse($form);
        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));

        var $validator = $form.validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);

            if ($element.hasClass('js-select2')) {
                $element.next().find('.select2-selection').addClass('select2-has-error');
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('js-select2')) {
                $element.next().find('.select2-selection').removeClass('select2-has-error');
            }
        };

        $form.find('.js-select2').on('change', function () {
            var $this = $(this);
            var $select2 = $this.next().find('.select2-selection');

            if (!$.isEmptyObject($validator.submitted) && $this.val() && $select2.hasClass('select2-has-error')) {
                $select2.removeClass('select2-has-error');
                $this.valid();
            }
        });
    }

    function registerEventClickIconEditItem() {
        $('.js-iconEditItem').off('click').on('click', function () {
            var fieldId = $(domElement.SelectedField).val();
            var itemId = $(this).data('item-id');
            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + $(this).data('url-update'),
                dataType: 'html',
                data: {
                    "fieldId": fieldId,
                    "id": itemId
                },
                success: function (data) {
                    $('#editItemModalContent').html(data);
                    initValidationForm($('#editItemForm'));
                    $('#editItemModal').modal('show');
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventClickCreateNewItem() {
        $('.js-createNewItem').on('click', function () {
            var $createItemModalContent = $(domElement.CreateItemModalContent);
            var fieldId = $(domElement.SelectedField).val();

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + $(this).data('url-create'),
                dataType: 'html',
                data: { "fieldId": fieldId },
                success: function (data) {
                    $createItemModalContent.html(data);

                    registerEventChangeCompanyBVI();
                    registerEventChangeDivision();
                    registerEventChangeDepartment();
                    registerEventChangeCostCenter();
                    registerEventChangeJobCategory();
                    registerEventChangeCountry();

                    var $createForm = $('#createItemForm');
                    initSelect2($createForm, '100%');
                    initValidationForm($createForm);

                    $(domElement.CreateItemModal).modal('show');
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

    function registerEventChangeJobCategory() {
        $(domElement.JobCategoryId).on('change', function () {
            //reset Grade Level
            var $gradeLevelId = $(domElement.GradeLevelId);
            $gradeLevelId.html(htmlDefaultOption);

            var jobCategoryId = $(this).val();
            if (jobCategoryId !== "" && $gradeLevelId.length > 0) {
                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'DataConfiguration/GetGradeLevelSelectListByJobCategoryId',
                    dataType: 'html',
                    data: { jobCategoryId: jobCategoryId },
                    success: function (data) {
                        $gradeLevelId.html(data);
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEventChangeCountry() {
        $(domElement.CountryId).on('change', function () {
            //reset City
            var $cityId = $(domElement.CityId);
            $cityId.html(htmlDefaultOption);

            var countryId = $(this).val();
            if (countryId !== "" && $cityId.length > 0) {
                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'DataConfiguration/GetCitySelectListByCountryId',
                    dataType: 'html',
                    data: { countryId: countryId },
                    success: function (data) {
                        $cityId.html(data);
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function reloadTableListItem() {
        var $fieldId = $(domElement.FieldId);
        var $selectedField = $(domElement.SelectedField);

        $fieldId.val($selectedField.val());
        $(domElement.FilterFieldForm).trigger('submit');
    }

    function onCreateNewItemSuccess(response) {
        var $createItemForm = $('#createItemForm');
        if (response.isSuccessful) {
            $("#createItemModal .close").trigger('click');
            $('.modal-backdrop').remove();
            commonFunctions.showSuccessMessage($('.msg-success'), response.message);
            reloadTableListItem();
        } else {
            if (response.errors) {
                $createItemForm.validate().showErrors(response.errors); //todo test here
            } else {
                commonFunctions.showErrorMessage($('.msg-error'), response.message);
            }
        }
    }

    function onUpdateItemSuccess(response) {
        var $editItemForm = $('#editItemForm');
        if (response.isSuccessful) {
            $("#editItemModal .close").trigger('click');
            $('.modal-backdrop').remove();
            commonFunctions.showSuccessMessage($('.msg-success'), response.message);
            reloadTableListItem();
        } else {
            if (response.errors) {
                $editItemForm.validate().showErrors(response.errors); //todo test here
            } else {
                commonFunctions.showErrorMessage($('.msg-error'), response.message);
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

    function initSelect2($form, width) {
        $form.find('.js-search-suggestion').select2({
            width: width,
            placeholder: '--Please select--'
        });
    }

    function initModule() {
        registerEventShowListItem();
        initSelect2($(domElement.FilterFieldForm), '250px');
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