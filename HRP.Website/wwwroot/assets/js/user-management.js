var userManagement = (function ($) {
    $('#user-table').dataTable({
        'ordering': false,
        'lengthMenu': [[20, 50, 100], [20, 50, 100]],
        'dom': '<"#toolbar.pull-left">ft<".clearfix"<".pull-left" l><".pull-right" ip>>',
        'info': false,
        'language': {
            'lengthMenu': 'Page size _MENU_',
            'search': ''
        },

    });

    var button = $('#btn-create-user');
    button.clone().removeClass('hidden').prependTo('#toolbar');
    button.addClass('hidden');

    $(".toggle-user-status").on('click', function (e) {
        e.preventDefault();
        $('#user-status').val(this.checked);
        $("#user-id").val($(this).data('user-id'));
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