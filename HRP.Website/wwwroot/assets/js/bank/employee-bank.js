var dataTable = {
    init: function () {
        $(".data-table").dataTable({
            "paging": false,
            "columnDefs": [{
                "orderable": false,
                "searchable": false,
                "targets": [0, -1]
            }],
            "order": [[1, "asc"]],
            'dom': 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            'info': false,
            "language": {
                "zeroRecords": "There is no data"
            }
        });

        dataTable.assignSearchButton();
    },

    assignSearchButton: function () {
        $("#quick-search").off("keyup").on("keyup", function () {
            $(".data-table").DataTable().search($(this).val()).draw();
        });
    }
}

$(document).ready(function () {
    dataTable.init();
});