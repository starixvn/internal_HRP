var dataConfiguration = (function ($) {
    var $filterFieldForm;

    function initModule() {
        $filterFieldForm = $('#filterFieldForm');
        registerEventShowListItem();
    }

    function initDataTable() {
        $('#tableItems').dataTable({
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
    }

    function registerEventQuickSearch() {
        $('#quickSearch').on('keyup', function () {
            $('#tableItems').DataTable().search($(this).val()).draw();
        });
    }

    function registerEventShowListItem() {
        $filterFieldForm.on('submit', function (e) {
            e.preventDefault();

            var fieldId = $('#fieldId').val();
            var getAllUrl = $('.field-' + fieldId).data('get-all-url');

            $.ajax({
                type: 'GET',
                dataType: 'html',
                url: commonFunctions.rootURL + getAllUrl,
                success: onGetSuccess,
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function onGetSuccess(response) {
        $('#tableWrapper').html(response);
        initDataTable();
        registerEventQuickSearch();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    dataConfiguration.initModule();
});