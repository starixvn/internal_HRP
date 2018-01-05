'use strict';

var createEmployment = (function ($) {
    var $employmentHeaderTab, $employmentTab, $createEmploymentForm,
        $bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement,
        defaultOption = '<option value="0" style="display: none;">--Please select--</option>';

    function registerEventChangeBVICompany() {
        $bviCompanyElement.on('change', function () {
            var companyId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetDivisionByCompanyId',
                dataType: 'json',
                data: {
                    companyId: companyId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].divisionId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $divisionElement.html(htmlOptions);
                    $divisionElement.select2('destroy').select2();
                    // reset
                    $departmentElement.html(defaultOption);
                    $sectionElement.html(defaultOption);
                    $costCenterElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeDivision() {
        $divisionElement.on('change', function () {
            var divisionId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetDepartmentByDivisionId',
                dataType: 'json',
                data: {
                    divisionId: divisionId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].departmentId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $departmentElement.html(htmlOptions);
                    $departmentElement.select2('destroy').select2();
                    // reset
                    $sectionElement.html(defaultOption);
                    $costCenterElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeDepartment() {
        $departmentElement.on('change', function () {
            var departmentId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetSectionByDepartmentId',
                dataType: 'json',
                data: {
                    departmentId: departmentId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].sectionId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $sectionElement.html(htmlOptions);
                    $sectionElement.select2('destroy').select2();
                    // reset
                    $costCenterElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeSection() {
        $sectionElement.on('change', function () {
            var sectionId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetCostCenterBySectionId',
                dataType: 'json',
                data: {
                    sectionId: sectionId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].costCenterId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $costCenterElement.html(htmlOptions);
                    $costCenterElement.select2('destroy').select2();
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function initSelect2() {
        $employmentTab.find('select.search-suggestion').select2();
    }

    function initDatepicker() {
        $employmentTab.find('input.datetimepicker').each(function () {
            var $this = $(this),
                mindate = $this.data('mindate'),
                maxdate = $this.data('maxdate');

            $this.datetimepicker({
                format: 'MMM DD, YYYY',
                minDate: mindate,
                maxDate: maxdate
            });

            $this.val('');
        });
    }

    function reparseform() {
        $createEmploymentForm.removeData('validator').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse('#createEmploymentForm');
    }

    function loadEmploymentForm() {
        $employmentHeaderTab.on('click', function () {
            var $this = $(this);
            var isFirstLoad = $this.data('isfirstload');

            if (isFirstLoad) {
                $this.data('isfirstload', false);

                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'EmployeeManagement/CreateEmployment',
                    dataType: 'html',
                    data: {
                        employeeId: 1
                    },
                    success: function (data) {
                        $employmentTab.html(data);

                        $createEmploymentForm = $('#createEmploymentForm');
                        $bviCompanyElement = $('#BVICompanyId');
                        $divisionElement = $('#DivisionId');
                        $departmentElement = $('#DepartmentId');
                        $sectionElement = $('#SectionId');
                        $costCenterElement = $('#CostCenterId');

                        reparseform();
                        initDatepicker();
                        initSelect2();

                        registerEventChangeBVICompany();
                        registerEventChangeDivision();
                        registerEventChangeDepartment();
                        registerEventChangeSection();
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function initModule() {
        $employmentTab = $('#tab-employment');
        $employmentHeaderTab = $('#tab-header-employment');

        loadEmploymentForm();
    }

    return {
        InitModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.InitModule();
});