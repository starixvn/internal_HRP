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
            columnDefs: [
                { className: 'col-width-58', orderable: true, targets: 0 },
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
                { className: 'col-width-50', orderable: false, targets: 20 },
            ],
            scrollX: true,
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

                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                return false;
            }
        });

        $('#SearchEmployee').keyup(function () {
            if (!$(this).val()) {
                $('#EmployeeId').val('');
            }
        });
    }

    function initModule() {
        companyId = parseInt($('#CompanyId').val(), 10);
        $btnBack = $('.js-btnBack');
        $table = $('#monthly-payroll-table');
        initSelect2();
        searchEmployeeSuggestion();
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