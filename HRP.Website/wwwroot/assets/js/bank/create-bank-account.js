var formhandler = {
    init: function () {
        formhandler.checkFormError();

        //$("form").on("submit input change keyup click",
        //    function() {
        //        formhandler.checkFormError();
        //    });

        formhandler.mutationObserve();
    },

    mutationObserve: function() {
        var config = { attributes: true, childList: true };

        var targetNode = $("form")[0];

        // Callback function to execute when mutations are observed
        var callback = function (mutationsList) {
            mutationsList.forEach(function (object) {
                if (object.type === "childList" || object.type === "attributes") {
                    formhandler.checkFormError();
                };
            });
        };

        // Create an observer instance linked to the callback function
        var observer = new MutationObserver(callback);

        // Start observing the target node for configured mutations
        observer.observe(targetNode, config);
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