'use strict';

var monthlyPayroll = (function ($) {
    var $btnBack, $table;

    function initSelect2() {
        $('.js-select2').select2({
            minimumResultsForSearch: Infinity
        });
    }

    function initDataTable() {
        $('#monthly-payroll-table').dataTable({
            ordering: false,
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

    function initModule() {
        $btnBack = $('.js-btnBack');
        $table = $('#monthly-payroll-table');
        initSelect2();
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