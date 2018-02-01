var userManagement = (function ($) {
    function initDataTable() {
        $('#user-table').dataTable({
            'ordering': false,
            'lengthMenu': [[20, 50, 100], [20, 50, 100]],
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

        $('#user-table_length select').select2({
            placeholder: '',
            minimumResultsForSearch: Infinity,
            width: '70px'
        });
    }

    function initModule() {
        $('.js-select2').select2({
            placeholder: '',
            minimumResultsForSearch: Infinity,
            width: '92px'
        });

        initDataTable();
    };

    return {
        onSearchSuccess: function (response, status) {
            initDataTable();
        },
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    $('#quick-search').off('keyup').on('keyup', function () {
        $('#user-table').DataTable().search($(this).val()).draw();
    });

    userManagement.initModule();
});