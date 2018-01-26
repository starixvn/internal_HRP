////var statehandler = {
////    init: function () {
////        // Load form data from session storage


////        // Save form data to session storage on every submit
////        $(body).on("submit",
////            "form.save-state",
////            function() {
////                // deserialize form values
////                var formData = $(this).deserialize();

////                // get form action to create key
////                var action = $(this).attr("action");

////                // save formData to sessionState
////                sessionStorage.setItem(action, formData);
////            });
////    }
////}

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
    ////statehandler.init();
    flow.init();
});