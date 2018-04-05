'use strict';

var payslips = (function ($) {
    var dataTable, $btnSendPayslip;

    function initSelect2() {
        $('.js-select2').select2({
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
        dataTable = $('#payslip-table').DataTable({
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

    function registerEvents() {
        $('#quick-search').off('keyup').on('keyup', function () {
            $('#payslip-table').DataTable().search($(this).val()).draw();
        });

        dataTable.on('order.dt search.dt', function () {
            dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $('.js-checkAll').on('click', function () {
            var rows = dataTable.rows({ 'search': 'applied' }).nodes();

            $('input.js-checkbox', rows).prop('checked', this.checked);
        });

        $('#payslip-table tbody').on('change', 'input.js-checkbox', function () {
            if (!this.checked) {
                var el = $('.js-checkAll').get(0);
                if (el && el.checked) {
                    el.checked = false;
                }
            }
        });

        $btnSendPayslip.on('click', function () {
            if (dataTable.$('input.js-checkbox:checked').length > 0) {
                $('#modal').modal('show');
            }
        });

        $('#send-payslip-form').on('submit', function (e) {
            var form = this;
            $(form).find('input.js-payrollId').remove();

            dataTable.$('input.js-checkbox:checked').each(function (index, element) {
                $(form).append(
                    $('<input>')
                        .attr('type', 'hidden')
                        .attr('name', 'PayrollIds[' + index + ']')
                        .attr('class', 'js-payrollId')
                        .val(this.value)
                );
            });
        });
    }

    function initModule() {
        $btnSendPayslip = $('.js-btnSendPayslip');
        initSelect2();
        initDatetimepicker();
        registerEvents();
    }

    return {
        initModule: initModule,
        initDataTable: initDataTable,
        onSendPayslipSuccess: function (response) {
            $('#modal').modal('hide');
            if (response.isSuccessful) {
                commonFunctions.showSuccessMessage($('.msg-success'));
                dataTable.$('input.js-checkbox:checked').each(function () {
                    $(this).closest('tr').find('.js-sent').removeClass('hidden');
                });
            } else {
                commonFunctions.showErrorMessage($('.msg-error'), response.message);
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    payslips.initModule();
});

if (document.readyState === 'loading') {
    payslips.initDataTable();
}