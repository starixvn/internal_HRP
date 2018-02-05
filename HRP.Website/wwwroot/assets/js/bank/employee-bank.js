var multiselect = {
    init: function () {
        var selectElements = $(".fanex-select");
        selectElements.each(function () {
            var options = multiselect.getItemsList($(this));
            $(this).fanexMultiSelect({
                listData: options,
                useCustomCheckAllText: true,
                customCheckAllText: "All"
            });

            multiselect.replaceNameAttrOfItem($(this));
        });

        // $("select").ultraselect({
        //     selectAllText: "All"
        // });
    },

    replaceNameAttrOfItem: function (selectElement) {
        var modelName = $(selectElement).prop("name");
        var inputs = $("input[data-item-id='" + modelName + "'");
        inputs.each(function () {
            $(this).attr("name", modelName);
        });
    },

    getItemsList: function (selectElement) {
        if ($(selectElement).is("select")) {
            if ($(selectElement).prop("multiple")) {
                // select element with multiple prop
                var values = [];
                $.each($(selectElement).prop("options"), function () {
                    var item = {
                        id: $(selectElement).prop("name"),
                        value: this.value,
                        text: this.text,
                        checked: this.selected,
                        enable: true,
                        cssClass: "-option-check"
                    }
                    values.push(item);
                });

                return values;
            }
        }

        return null;
    }
};

var dataTable = {
    init: function () {
        $(".data-table").dataTable({
            "paging": false,
            "columnDefs": [{
                "orderable": false,
                "searchable": false,
                "targets": [0, -1]
            }],
            "order": [[2, "asc"]],
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
    multiselect.init();
    dataTable.init();
});