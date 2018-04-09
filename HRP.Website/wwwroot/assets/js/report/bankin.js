var bankin = {
    init: function() {
        bankin.initSelect2();
        bankin.initDatetimepicker();
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
    }
}

$(document).ready(function() {
    bankin.init();
});