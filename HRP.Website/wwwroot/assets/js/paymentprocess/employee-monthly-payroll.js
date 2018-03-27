'use strict';

var monthlyPayroll = (function ($) {
    var $btnBack, $table, companyId;

    function initSelect2() {
        $('.js-select2').select2({
            minimumResultsForSearch: Infinity,
            width: '92px'
        });
    }

    function initDataTable() {
        $('#monthly-payroll-table').dataTable({
            ordering: true,
            order: [[1, 'asc']],
            lengthMenu: [commonFunctions.pagerRanges, commonFunctions.pagerRanges],
            dom: 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            info: false,
            language: {
                lengthMenu: 'Page size _MENU_',
                search: '',
                paginate: {
                    first: 'First',
                    last: 'Last',
                    next: '<i class="icon-next"></i>',
                    previous: '<i class="icon-back"></i>'
                },
                zeroRecords: commonFunctions.noDataOnTableMsg,
                sZeroRecords: commonFunctions.noDataOnTableMsg
            },
            aoColumnDefs: [
                { className: 'col-width-58', orderable: true, targets: 0, sType: "sortByStatus", bSortable: true },
                { orderable: true, targets: 1 },
                { orderable: true, targets: 2 },
                { orderable: true, targets: 3 },
                { className: 'col-width-76', orderable: true, targets: 4 },
                { orderable: false, targets: 5 },
                { orderable: false, targets: 6 },
                { orderable: false, targets: 7 },
                { orderable: false, targets: 8 },
                { orderable: false, targets: 9 },
                { orderable: false, targets: 10 },
                { orderable: false, targets: 11 },
                { orderable: false, targets: 12 },
                { orderable: false, targets: 13 },
                { orderable: false, targets: 14 },
                { orderable: false, targets: 15 },
                { orderable: false, targets: 16 },
                { orderable: false, targets: 17 },
                { orderable: false, targets: 18 },
                { className: 'col-width-60', orderable: false, targets: 19 },
                { className: 'col-width-50', orderable: false, targets: 20 }
            ],
            scrollX: true,
            scrollY: '50vh',
            scrollCollapse: true,
            responsive: true,
            fixedColumns: {
                leftColumns: 5,
                rightColumns: 2
            }
        });

        $('select[name=monthly-payroll-table_length]').select2({
            minimumResultsForSearch: Infinity
        });
    }

    function searchEmployeeSuggestion() {
        $('#SearchEmployee').autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');
                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: commonFunctions.rootURL + 'PaymentProcess/GetEmployeeSuggestionByName',
                    dataType: 'json',
                    data: {
                        companyId: companyId,
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
                $('#SearchEmployee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
                $('#Employee').val(ui.item.label);

                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                return false;
            }
        });

        $('#SearchEmployee').keyup(function () {
            $('#Employee').val($(this).val());

            if (!$(this).val()) {
                $('#EmployeeId').val('');
            }
        });

        $('#SearchIDNo').autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');
                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: commonFunctions.rootURL + 'PaymentProcess/GetEmployeeSuggestionByIDNo',
                    dataType: 'json',
                    data: {
                        companyId: companyId,
                        idNo: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.idNo,
                                    value: item.idNo
                                };
                            }));
                        }
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $('#SearchIDNo').val(ui.item.label);
                $('#IDNo').val(ui.item.label);

                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                return false;
            }
        });

        $('#SearchIDNo').keyup(function () {
            $('#IDNo').val($(this).val());
        });
    }

    function initContextMenu() {
        $.contextMenu({
            selector: '.js-contextMenu',
            trigger: 'left',
            build: function ($trigger, e) {
                var $element = $(e.currentTarget);
                var basicSalary = {
                    title: $element.next().find('.js-menuItem-basicSalary').html(),
                    url: $element.next().find('.js-menuItem-basicSalary').attr('href')
                };
                var allowance = {
                    title: $element.next().find('.js-menuItem-allowance').html(),
                    url: $element.next().find('.js-menuItem-allowance').attr('href')
                };

                return {
                    callback: function (key, options) {
                        switch (key) {
                            case 'BasicSalary':
                                location.href = basicSalary.url;
                                break;

                            case 'Allowances':
                                location.href = allowance.url;
                                break;
                        }
                    },
                    items: {
                        'BasicSalary': {
                            name: basicSalary.title,
                            isHtmlName: true
                        },
                        'Allowances': {
                            name: allowance.title,
                            isHtmlName: true
                        }
                    }
                };
            }
        });
    }

    function registerEventSubmitForm() {
        $('form.search-form').on('submit', function (e) {
            var costCenterIds = $("#CostCenterIds").val();

            if (!costCenterIds) {
                $('.list-message-error').removeClass('hidden');
                $('#CostCenterMultiSelect').addClass('validation-error');

                e.preventDefault();
            }
        });
    }

    function initModule() {
        companyId = parseInt($('#CompanyId').val(), 10);
        $btnBack = $('.js-btnBack');
        $table = $('#monthly-payroll-table');
        initSelect2();
        searchEmployeeSuggestion();
        initContextMenu();
        registerEventSubmitForm();

        $('#quick-search').off('keyup').on('keyup', function () {
            $table.DataTable().search($(this).val()).draw();
        });
    }

    return {
        initModule: initModule,
        initDataTable: initDataTable
    };
}(jQuery));

$(document).ready(function () {
    monthlyPayroll.initModule();

    $("#CostCenterMultiSelect").fanexMultiSelect({
        listData: jQuery.parseJSON($('#HierarchyCompanyJson').text()),
        useCustomCheckAllText: true,
        customCheckAllText: "All",
        onCheck: function () {
            var costCenterIds = $("#CostCenterMultiSelect").fanexMultiSelect('getItems', true).join(',');
            $("#CostCenterIds").val(costCenterIds);
        }
    });
});

if (document.readyState === 'loading') {
    monthlyPayroll.initDataTable();
}