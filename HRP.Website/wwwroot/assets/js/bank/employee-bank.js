var dataTable = {
    init: function () {
        dataTable.initDataTables();
    },

    initDataTables: function () {
        var table = $(".data-table").DataTable({
            "paging": false,
            "columnDefs": [{
                "orderable": false,
                "searchable": false,
                "targets": [0, -1]
            }],
            "order": [
                [1, "asc"]
            ],
            'dom': 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            'info': false,
            "language": {
                "zeroRecords": "There is no data"
            }
        });

        table.on('order.dt search.dt', function () {
            table.column(0, {
                search: 'applied',
                order: 'applied'
            }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

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