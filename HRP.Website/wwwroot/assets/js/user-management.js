var userManagement = (function ($) {
    $('#user-table').dataTable({
        'ordering': false,
        'lengthMenu': [[10 ,20, 50, 100], [10 ,20, 50, 100]],
        'dom': 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
        'info': false,
        'language': {
            'lengthMenu': 'Page size _MENU_',
            'search': '',
            'paginate': {
                'first': 'First',
                'last': 'Last',
                'next': '<i class="glyphicon glyphicon-menu-right"></i>',
                'previous': '<i class="glyphicon glyphicon-menu-left"></i>'
            },
        },

    });

    $('#quick-search').off('keyup').on('keyup', function () {
        $('#user-table').DataTable().search($(this).val()).draw();
    });

    $(".toggle-user-status").on('click', function (e) {
        e.preventDefault();
        $('#user-status').val(this.checked);
        $('#user-id').val($(this).data('user-id'));
        $('#user-status-modal').modal('show');
        return false;
    });

    return {
        onUpdateUserStatus: function (result, status) {
            $('.modal').modal('hide');
            $('.toggle-user-status', '#user-' + result.userId)[0].checked = result.isActive;
        }
    }
}(jQuery));