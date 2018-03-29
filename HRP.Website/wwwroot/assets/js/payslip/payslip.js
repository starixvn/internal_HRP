'use strict';

var payslips = (function ($) {
    function initSelect2() {
        $('.js-select2').select2({
            placeholder: '',
            minimumResultsForSearch: Infinity
        });
    }

    function initDatetimepicker() {
        $('.datetimepicker').datetimepicker({
            viewMode: 'months',
            format: 'MM-YYYY'
        });

        $('.icon-calendar:not(.disabled)').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').show();
        });
    }

    function initDataTable() {
        $('#payslip-table').dataTable({
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
                { orderable: false, targets: 0 },
                { orderable: true, targets: 1 },
                { orderable: true, targets: 2 },
                { orderable: true, targets: 3 },
                { orderable: true, targets: 4 },
                { orderable: false, targets: 5 },
                { orderable: false, targets: 6 },
                { orderable: true, targets: 7, sType: "sortByStatus", bSortable: true },
                { orderable: false, targets: 8 },
                { orderable: false, targets: 9 }
            ]
        });

        $('select[name=payslip-table_length]').select2({
            minimumResultsForSearch: Infinity
        });
    }

    function initModule() {
        initSelect2();
        initDatetimepicker();
    }

    return {
        initModule: initModule,
        initDataTable: initDataTable
    };
}(jQuery));

$(document).ready(function () {
    payslips.initModule();
});

if (document.readyState === 'loading') {
    payslips.initDataTable();
}