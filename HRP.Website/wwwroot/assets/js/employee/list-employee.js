'use strict';

var listEmployee = {
    init: function () {
        listEmployee.query();
        listEmployee.searchEmployeeSuggestion();
    },

    query: function () {
        var departmentIds = $("#DepartmentIds").val();
        if (!departmentIds) {
            $("#departmentIds-validation").removeClass("hidden");
            $("#CompanySelect").addClass("input-validation-error");
            return false;
        } else {
            $("#departmentIds-validation").addClass("hidden");
            $("#CompanySelect").remove("input-validation-error");
        }
        $.ajax({
            url: commonFunctions.rootURL + 'EmployeeManagement/EmployeeList',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#employee-list').html(response);
                listEmployee.initDataTable();
            },
            error: commonFunctions.handleAjaxError
        });
    },

    initDataTable: function () {
        $('#employee-table').dataTable({
            'ordering': true,
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
        $("#SearchEmployee").autocomplete({
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
                                    label: item.nickName + ' (' + item.fullName + ')',
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
                $("#SearchEmployee").val(ui.item.label);
                $("#EmployeeId").val(ui.item.value);
                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                $("#SearchEmployee").val(ui.item.label);
                return false;
            },
            change: function (event, ui) {
                var employeeName = $(this).val();
                if (!employeeName) $("#EmployeeId").val(""); 
            }
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
                    onCheck: function () {
                        var selectedValue = $("#CompanySelect").fanexMultiSelect('getItems', true).join(',')
                        var departmentIds = listDepartment.SetDepartmentIdsValue(selectedValue);
                        $("#DepartmentIds").val(departmentIds);
                    }
                });
            },
            error: commonFunctions.handleAjaxError
        });
    },

    GetDivisionIds: function (divisions, companyId, departments) {
        var value = [];
        for (var i = 0; i < divisions.length; i++) {
            if (divisions[i].companyId == companyId) {
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

    SetDepartmentIdsValue: function (selectedIds) {
        var selectedArr = selectedIds.split(",");
        var departmentArr = [];
        for (var i = 0; i < selectedArr.length; i++) {
            if (selectedArr[i].includes("d")) {
                var id = selectedArr[i].slice(1);
                departmentArr.push(id);
            }
        }
        return departmentArr.join();
    },

    GetDepartmentIds: function (departments, divisionId) {
        var value = [];
        for (var i = 0; i < departments.length; i++) {
            if (departments[i].divisionId == divisionId) {
                var department = {
                    value: 'd' + departments[i].departmentId,
                    text: departments[i].code,
                    checked: true,
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

$(document).ready(function () {
    listEmployee.init();
    listDepartment.init();
});

$('#quick-search').off('keyup').on('keyup', function () {
    $('#employee-table').DataTable().search($(this).val()).draw();
});

$('form.form-filter').submit(function (e) {
    e.preventDefault();
    listEmployee.query();
});