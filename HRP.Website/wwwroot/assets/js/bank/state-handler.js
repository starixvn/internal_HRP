var flow = {
    init: function() {
        $(".back").on("click",
            function (e) {
                e.preventDefault();
                window.history.back();
            });
    }
}

$(document).ready(function() {
    flow.init();
});