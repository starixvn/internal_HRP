var editUser = (function ($) {
    return {
        onEditBegin: function () {
            $('#btn-edit').button('loading')
        },
        onEditSuccess: function (result, status) {
            var alert = $('#edit-user-alert');

            alert.removeClass('hidden');

            window.setTimeout(function () {
                alert.addClass('hidden');
            }, 2000);
        },
        onEditComplete: function () {
            $('#btn-edit').button('reset');
        }
    };
}(jQuery));