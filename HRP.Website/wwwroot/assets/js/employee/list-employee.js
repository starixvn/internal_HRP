'use strict';

var employeeList = (function ($) {
    function initSelect2() {
        $("select[name=employee-table_length]").select2({
            minimumResultsForSearch: Infinity
        });
    }

    function initDataTable() {
        $('#employee-table').dataTable({
            'ordering': true,
            'order': [[1, "asc"]],
            "columns": [
                { "sortable": false },
                { "sortable": true },
                { "sortable": true },
                { "sortable": true },
                { "sortable": true },
                { "sortable": true },
                { "sortable": true },
                { "sortable": true },
                { "sortable": true },
                { "sortable": false }
            ],
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
                },
                'zeroRecords': commonFunctions.noDataOnTableMsg,
                'sZeroRecords': commonFunctions.noDataOnTableMsg
            }
        });

        initSelect2();
    }

    function searchEmployeeSuggestion() {
        $("#DisplayName").autocomplete({
            source: function (request, response) {
                var term = request.term;

                $.ajax({
                    url: commonFunctions.rootURL + 'Employee/GetEmployeeSuggestionByName',
                    dataType: 'json',
                    data: {
                        name: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.nickname + ' (' + item.fullName + ')',
                                    value: item.employeeId
                                };
                            }));
                        }
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $("#DisplayName").val(ui.item.label);
                $("#EmployeeId").val(ui.item.value);
                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                $("#DisplayName").val(ui.item.label);
                return false;
            },
            change: function (event, ui) {
                var employeeName = $(this).val();
                if (!employeeName) $("#EmployeeId").val("");
            }
        });
    }

    function validateSelection() {
        var departmentIds = $("#CompanyMultiSelect").fanexMultiSelect('getItems', true).join(',');
        var employeeStatusIds = $("#EmploymentStatusMultiSelect").fanexMultiSelect('getItems', true).join(',');
        if (departmentIds && employeeStatusIds) {
            return true;
        }

        return false;
    }

    function showErrorMessage(message, input) {
        $("#error-alert").removeClass("hidden");
        $(message).show();
        $(input).addClass("validation-error");
    }

    function hideErrorMessage(message, input) {
        if (validateSelection()) {
            $("#error-alert").addClass("hidden");
        }

        $(message).hide();
        $(input).removeClass("validation-error");
    }

    function showSelectError() {
        var departmentIds = $("#CompanyMultiSelect").fanexMultiSelect('getItems', true).join(',');
        var employeeStatusIds = $("#EmploymentStatusMultiSelect").fanexMultiSelect('getItems', true).join(',');

        if (!departmentIds) {
            showErrorMessage("#company-validation", "#CompanyMultiSelect");
        } else {
            hideErrorMessage("#company-validation", "#CompanyMultiSelect");
        }

        if (!employeeStatusIds) {
            showErrorMessage("#employment-status-validation", "#EmploymentStatusMultiSelect");
        } else {
            hideErrorMessage("#employment-status-validation", "#EmploymentStatusMultiSelect");
        }
    }

    function initModule() {
        searchEmployeeSuggestion();

        $('#quick-search').off('keyup').on('keyup', function () {
            $('#employee-table').DataTable().search($(this).val()).draw();
        });

        $("#DisplayName").keyup(function () {
            if (!$(this).val()) {
                $('#EmployeeId').val('');
            }
        });

        $("form").on("submit", function (e) {
            if (!validateSelection()) {
                e.preventDefault();

                showSelectError();
            }
        });
    }

    function initDepartment() {
        $.ajax({
            url: commonFunctions.rootURL + 'Employee/GetDepartment',
            dataType: 'json',
            data: {
                departmentIds: $('#DepartmentIds').val()
            },
            success: function (result) {
                $("#CompanyMultiSelect").fanexMultiSelect({
                    listData: result,
                    useCustomCheckAllText: true,
                    customCheckAllText: "All",
                    onCheck: function () {
                        var selectedValue = $("#CompanyMultiSelect").fanexMultiSelect('getItems', true).join(',');
                        $("#DepartmentIds").val(selectedValue);
                    }
                });
            }
        });
    }

    function initEmployeeStatus() {
        $.ajax({
            url: commonFunctions.rootURL + 'Employee/GetEmployeeStatus',
            dataType: 'json',
            data: {
                employmentStatusIds: $('#EmploymentStatusIds').val()
            },
            success: function (result) {
                $("#EmploymentStatusMultiSelect").fanexMultiSelect({
                    listData: result,
                    useCustomCheckAllText: true,
                    customCheckAllText: "All",
                    onCheck: function () {
                        var selectedValue = $("#EmploymentStatusMultiSelect").fanexMultiSelect('getItems', true).join(',');
                        $("#EmploymentStatusIds").val(selectedValue);
                    }
                });
            }
        });
    }

    return {
        initModule: initModule,
        initDepartment: initDepartment,
        initEmployeeStatus: initEmployeeStatus,
        initDataTable: initDataTable
    };
}(jQuery));

$(document).ready(function () {
    employeeList.initModule();

    employeeList.initDepartment();

    employeeList.initEmployeeStatus();
});

if (document.readyState === 'loading') {
    employeeList.initDataTable();
}