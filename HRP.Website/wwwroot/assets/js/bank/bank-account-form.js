var bankAccountForm = {
    init: function () {
        bankAccountForm.checkFormError();
        bankAccountForm.mutationObserve();
        bankAccountForm.assignRevalidate();
    },

    assignRevalidate: function() {
        $("select").on("change",
            function () {
                $(this).valid();
                $(".revalidate").each(function () {
                    $(this).removeData("previousValue");
                    $(this).valid();
                });
            });
    },

    config: { attributes: true, childList: true },

    waitTime: 5000,

    countTimeInSeconds: 5,

    callbank: function (mutationsList) {
        // Callback function to execute when mutations are observed
        mutationsList.forEach(function (object) {
            if (object.type === "childList" || object.type === "attributes") {
                bankAccountForm.checkFormError();
            };
        });
    },

    mutationObserve: function () {
        var targetNode = $(".bank-account-form")[0];

        // Callback function to execute when mutations are observed
        var callback = function (mutationsList) {
            mutationsList.forEach(function (object) {
                if (object.type === "childList" || object.type === "attributes") {
                    bankAccountForm.checkFormError();
                };
            });
        };

        // Create an observer instance linked to the callback function
        var observer = new MutationObserver(callback);

        // Start observing the target node for configured mutations
        if (targetNode != undefined) {
            observer.observe(targetNode, bankAccountForm.config);
        }
    },

    checkFormError: function () {
        if ($(".bank-account-form").find(".field-validation-error").length === 0) {
            // Valid
            $(".bank-account-form .list-message-error").addClass("hidden");
        } else {
            $(".bank-account-form .list-message-error").removeClass("hidden");
        }
    },

    onSaveBegin: function() {
        $(".bank-account-form").find(":submit").button("loading");
    },

    onSaveComplete: function() {
        $(".bank-account-form").find(":submit").button("reset");
    },

    onSaveSuccess: function (response, status) {
        if (response.isSuccessful) {
            $(".bank-account-form .msg-error").text("").addClass("hidden");
            $(".bank-account-form .msg-success").removeClass("hidden");

            if ($(".bank-account-form").data("formstyle") === "Page") {

                //// Reload page
                //location.reload();

                // Disable form
                bankAccountForm.disableForm();

                setInterval(bankAccountForm.countdown, 1000);

                setTimeout(function () {
                    window.location.href = response.redirectTo;
                    //$(".bank-account-form .msg-success").text("").addClass("hidden");
                }, bankAccountForm.waitTime);
            }

            if ($(".bank-account-form").data("formstyle") === "Modal") {
                // Disable form
                bankAccountForm.disableForm();

                setInterval(bankAccountForm.countdown, 1000);

                setTimeout(function () {
                    $(".bank-account-form").closest(".modal").modal("hide");
                }, bankAccountForm.waitTime);
            }
        } else {
            if (response.message) {
                $(".bank-account-form .msg-error").text("");
                $(".bank-account-form .msg-error").text(response.message).removeClass("hidden");

                ////setTimeout(function () {
                ////    $(".bank-account-form .msg-error").text("").addClass("hidden");
                ////}, bankAccountForm.waitTime);
            }
        }
    },

    onAjaxFailure: function() {

    },

    countdown: function() {
        bankAccountForm.countTimeInSeconds = bankAccountForm.countTimeInSeconds - 1;
        if (bankAccountForm.countTimeInSeconds <= 0) {
            clearInterval(bankAccountForm.countdown);
            //counter ended, do something here
            return;
        }

        $(".bank-account-form .count-down-remaining").text(bankAccountForm.countTimeInSeconds);
    },

    disableForm: function() {
        $(".bank-account-form :input").attr("disabled", true).addClass("disabled");
        $(".bank-account-form button").attr("disabled", true).addClass("disabled");
        $(".bank-account-form .button").attr("disabled", true).addClass("disabled");
    }
}

$(document).ready(function () {
    bankAccountForm.init();
});