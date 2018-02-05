var userManagement = (function ($) {
    function initDataTable() {
        $('#user-table').dataTable({
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
                },
                'zeroRecords': commonFunctions.noDataOnTableMsg,
                'sZeroRecords': commonFunctions.noDataOnTableMsg
            }
        });

        $('#user-table_length select').select2({
            placeholder: '',
            minimumResultsForSearch: Infinity
        });
    }

    function initModule() {
        $('.js-select2').select2({
            placeholder: '',
            minimumResultsForSearch: Infinity
        });
    };

    return {
        initModule: initModule,
        initDataTable: initDataTable
    };
}(jQuery));

$(document).ready(function () {
    $('#quick-search').off('keyup').on('keyup', function () {
        $('#user-table').DataTable().search($(this).val()).draw();
    });

    userManagement.initModule();
});

if (document.readyState === 'loading') {
    userManagement.initDataTable();
}