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

var transformSelect = {
    init: function () {
        $("select").select2({
            minimumResultsForSearch: Infinity
        });

        $("select").on("change",
            function () {
                $(this).valid();
                $(".revalidate").each(function() {
                    $(this).removeData("previousValue");
                    $(this).valid();
                });
            });

        //$(".select2").select2().change(function () {
        //    $(this).valid();
        //});
    }
}

$(document).ready(function () {
    //multiselect.init();
    transformSelect.init();
});