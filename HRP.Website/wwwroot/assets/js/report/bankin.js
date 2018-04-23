var bankin = {
    init: function() {
        bankin.initSelect2();
        bankin.initDatetimepicker();
        bankin.initPaymentTransactionFanexMultiSelect();
    },

    initSelect2: function() {
        $(".js-select2").select2({
            minimumResultsForSearch: Infinity
        });
    },

    initDatetimepicker: function() {
        $(".datetimepicker").datetimepicker({
            viewMode: "months",
            format: "MM-YYYY"
        });

        $(".icon-calendar:not(.disabled)").on("click",
            function() {
                $(this).siblings(".datetimepicker").data("DateTimePicker").show();
            });
    },

    initPaymentTransactionFanexMultiSelect: function() {
        $.ajax({
            url: $("#get-payment-transactions-url").attr("href"),
            success: function(result) {
                $("#payment-transactions-multiselect").fanexMultiSelect({
                    listData: result,
                    useCustomCheckAllText: true,
                    customCheckAllText: "All",
                    onCheck: function() {
                        var selectedValue =
                            $("#payment-transactions-multiselect").fanexMultiSelect("getItems", true).join(",");
                        $("#selected-payment-transactions").val(selectedValue);
                    }
                });

                var selectedValue =
                    $("#payment-transactions-multiselect").fanexMultiSelect("getItems", true).join(",");
                $("#selected-payment-transactions").val(selectedValue);
            }
        });
    }
}

$(document).ready(function() {
    bankin.init();
});