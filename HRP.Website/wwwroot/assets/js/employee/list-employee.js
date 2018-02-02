'use strict';

var listEmployee = {
    init: function () {
        listEmployee.initDataTable();
        listEmployee.searchEmployeeSuggestion();
        listEmployee.initSelect2();
    },

    getValueIsSelected: function (inputIds, selectId) {
        var idsValue = inputIds.split(",");
        for (var i = 0; i < idsValue.length; i++) {
            if (idsValue[i] === selectId) return true;
        }
        return false;
    },

    showErrorMessage: function (message, input) {
        $("#error-alert").removeClass("hidden");
        $(message).show();
        $(input).addClass("validation-error");
    },

    hideErrorMessage: function (message, input) {
        if (listEmployee.validateSelection()) {
            $("#error-alert").addClass("hidden");
        }
        $(message).hide();
        $(input).removeClass("validation-error");
    },

    showSelectError: function () {
        var departmentIds = $("#DepartmentIds").val();
        var employeeStatusIds = $("#EmployeeStatusIds").val();
        console.log(departmentIds);
        console.log(employeeStatusIds);

        if (!departmentIds) {
            listEmployee.showErrorMessage("#company-validation", "#CompanySelect");
        }
        if (departmentIds) {
            listEmployee.hideErrorMessage("#company-validation", "#CompanySelect");
        }
        if (!employeeStatusIds) {
            listEmployee.showErrorMessage("#employment-status-validation", "#EmployeeStatus");
        }
        if (employeeStatusIds) {
            listEmployee.hideErrorMessage("#employment-status-validation", "#EmployeeStatus");
        }
    },

    validateSelection: function () {
        var departmentIds = $("#DepartmentIds").val();
        var employeeStatusIds = $("#EmployeeStatusIds").val();
        if (departmentIds && employeeStatusIds) {
            return true;
        }
        return false;
    },

    initDataTable: function () {
        $('#employee-table').dataTable({
            'ordering': true,
            'order': [[1, "asc"]],
            "columns": [
                {
                    "sortable": false
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": true
                },
                {
                    "sortable": false
                }
            ],
            'lengthMenu': [[50, 100, 500], [50, 100, 500]],
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
    },

    searchEmployeeSuggestion: function () {
        $("#DisplayName").autocomplete({
            source: function (request, response) {
                var term = request.term;

                $.ajax({
                    url: commonFunctions.rootURL + 'EmployeeManagement/GetEmployeeSuggestionByName',
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
    },

    initSelect2: function () {
        $("select[name=employee-table_length]").select2({
            minimumResultsForSearch: Infinity
        });
    }
};

var listDepartment = {
    init: function () {
        $.ajax({
            url: commonFunctions.rootURL + 'EmployeeManagement/GetCompanyTree',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                var companyTree = listDepartment.buildCompanyTree(res.companies, res.divisions, res.departments);
                $("#CompanySelect").fanexMultiSelect({
                    listData: companyTree,
                    useCustomCheckAllText: true,
                    customCheckAllText: "All",
                    onCheck: function () {
                        var selectedValue = $("#CompanySelect").fanexMultiSelect('getItems', true).join(',');
                        $("#DepartmentIds").val(selectedValue);
                    }
                });
            },
            error: commonFunctions.handleAjaxError
        });
    },

    GetDivisionIds: function (divisions, companyId, departments) {
        var value = [];
        for (var i = 0; i < divisions.length; i++) {
            if (divisions[i].companyId === companyId) {
                var division = {
                    value: divisions[i].divisionId,
                    text: divisions[i].code,
                    checked: true,
                    enable: true,
                    cssClass: "-option-check",
                    items: listDepartment.GetDepartmentIds(departments, divisions[i].divisionId)
                };
                value.push(division);
            }
        }
        return value;
    },

    GetDepartmentIds: function (departments, divisionId) {
        var value = [];
        for (var i = 0; i < departments.length; i++) {
            if (departments[i].divisionId === divisionId) {
                var department = {
                    value: departments[i].departmentId,
                    text: departments[i].code,
                    checked: listEmployee.getValueIsSelected($("#DepartmentIds").val(), departments[i].departmentId),
                    enable: true,
                    cssClass: "-option-check",
                };
                value.push(department);
            }
        }
        return value;
    },

    buildCompanyTree: function (companies, divisions, departments) {
        var value = [];
        for (var i = 0; i < companies.length; i++) {
            var company = {
                value: companies[i].companyId,
                text: companies[i].code,
                checked: true,
                enable: true,
                cssClass: "-option-check",
                items: listDepartment.GetDivisionIds(divisions, companies[i].companyId, departments)
            }
            value.push(company);
        }
        return value;
    },
};

var listEmployeeStatus = {
    getItemsList: function (selectElement) {
        var values = [];
        $.each($(selectElement).prop("options"), function () {
            var item = {
                id: $(selectElement).prop("name"),
                value: this.value,
                text: this.text,
                checked: listEmployee.getValueIsSelected($("#EmployeeStatusIds").val(), this.value),
                enable: true,
                cssClass: "-option-check"
            }
            values.push(item);
        });
        return values;
    },

    init: function () {
        $("#EmployeeStatus").fanexMultiSelect({
            listData: listEmployeeStatus.getItemsList($("#EmployeeStatus")),
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            onCheck: function () {
                var selectedValue = $("#EmployeeStatus").fanexMultiSelect('getItems', true).join(',')
                $("#EmployeeStatusIds").val(selectedValue);
            }
        });
    }
};
$(document).ready(function () {
    listEmployee.init();
    listDepartment.init();
    listEmployeeStatus.init();
});
$('#quick-search').off('keyup').on('keyup', function () {
    $('#employee-table').DataTable().search($(this).val()).draw();
});

$("#DisplayName").keyup(function () {
    if (!$(this).val()) {
        $('#EmployeeId').val('');
    }
});

$("form").on("submit", function (e) {
    if (!listEmployee.validateSelection()) {
        e.preventDefault();
        listEmployee.showSelectError();
    }
});