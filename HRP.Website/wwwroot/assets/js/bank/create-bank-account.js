var formhandler = {
    init: function () {
        $("form").on("submit input change keyup",
            function () {
                formhandler.checkFormError();
            });
    },

    checkFormError: function () {
        if ($("form").find(".field-validation-error").length === 0) {
            // Valid
            $(".list-message-error").addClass("hidden");
        } else {
            $(".list-message-error").removeClass("hidden");
        }
    }
}

$(document).ready(function () {
    formhandler.init();
});