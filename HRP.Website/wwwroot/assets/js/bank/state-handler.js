var flow = {
    init: function() {
        $(".back").on("click",
            function (e) {
                e.preventDefault();
                if (!$(this).hasClass("disabled")) {
                    window.history.back();
                }
            });
    }
}

$(document).ready(function() {
    flow.init();
});