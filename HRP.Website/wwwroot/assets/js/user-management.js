(function ($) {
    $('#user-table').dataTable({
        'ordering': false,
        'lengthMenu': [[20, 50, 100], [20, 50, 100]],
        'dom': '<f<t><".clearfix"<".pull-left" l><".pull-right" ip>>>',
        'info': false,
        'language': {
            'lengthMenu': 'Page size _MENU_',
            'search': ''
        }
    });
}(jQuery));