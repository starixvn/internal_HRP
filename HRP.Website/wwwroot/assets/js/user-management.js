var userManagement = (function ($) {
    var init = function () {
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
                },
            },

        });
    }

    $('#quick-search').off('keyup').on('keyup', function () {
        $('#user-table').DataTable().search($(this).val()).draw();
    });

    return {
        onSearchSuccess: function (response, status) {
            init();
        },
        init: init
    }

}(jQuery));

userManagement.init();