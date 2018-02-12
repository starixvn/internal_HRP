var transformSelect = {
    init: function () {
        $("select").select2({
            minimumResultsForSearch: Infinity
        });
    }
}

$(document).ready(function () {
    transformSelect.init();
});