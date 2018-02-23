var bankAccountForm = {
    init: function () {
        bankAccountForm.handleFormSubmittedInModal();
        bankAccountForm.handleFormSuccessed();
        bankAccountForm.checkFormError();
        bankAccountForm.mutationObserve();
    },

    handleFormSubmittedInModal: function () {

        $("#dynamic-modals").on("hidden.bs.modal",
            ".modal",
            function () {
                $(this).remove();
                $(".modal-backdrop").remove();
            });

        var isModal = $(".bank-account-form :submit").data("ismodal") === true;
        if (isModal) {
            $(".modal-bank-account-form").on("submit",
                "form",
                function (e) {
                    e.preventDefault();

                    $.validator.unobtrusive.parse($(this));
                    $(this).validate();

                    $.ajax({
                        url: $(this).attr("action"),
                        method: $(this).attr("method"),
                        data: $(this).serialize(),
                        success: function (result) {
                            $(".modal-bank-account-form").modal("hide");
                            $("#dynamic-modals").html(result);

                            $("#dynamic-modals select").select2({
                                width: "100%",
                                minimumResultsForSearch: Infinity
                            });

                            bankAccountForm.init();

                            var form = $("#dynamic-modals form")
                                .removeData("validator") /* added by the raw jquery.validate plugin */
                                .removeData("unobtrusiveValidation"); /* added by the jquery unobtrusive plugin*/

                            $.validator.unobtrusive.parse(form);

                            $(".modal-bank-account-form").modal();
                        }
                    });
                });
        }
    },

    handleFormSuccessed: function () {
        var isSuccessful = $("#is-successful").val();
        var isModal = $(".bank-account-form :submit").data("ismodal") === true;
        if (isSuccessful) {
            bankAccountForm.disableForm();

            setInterval(bankAccountForm.countdown, 1000);

            setTimeout(function () {
                if (isModal) {
                    $(".bank-account-form").closest(".modal").modal("hide");
                } else {
                    window.location.href = $("#index-link").prop("href");
                }
            },
                bankAccountForm.waitTime);
        }
    },

    config: { attributes: true, childList: true },

    waitTime: 5000,

    countTimeInSeconds: 5,

    mutationObserve: function () {
        var targetNode = $(".bank-account-form")[0];

        // Callback function to execute when mutations are observed
        var callback = function (mutationsList) {
            mutationsList.forEach(function (object) {
                if (object.type === "childList" || object.type === "attributes") {
                    bankAccountForm.checkFormError();
                }
            });
        };

        // Create an observer instance linked to the callback function
        var observer = new MutationObserver(callback);

        // Start observing the target node for configured mutations
        if (targetNode !== undefined) {
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

    countdown: function () {
        bankAccountForm.countTimeInSeconds = bankAccountForm.countTimeInSeconds - 1;
        if (bankAccountForm.countTimeInSeconds <= 0) {
            clearInterval(bankAccountForm.countdown);
            //counter ended, do something here
            return;
        }

        $(".bank-account-form .count-down-remaining").text(bankAccountForm.countTimeInSeconds);
    },

    disableForm: function () {
        $(".bank-account-form :input").attr("disabled", true).addClass("disabled");
        $(".bank-account-form button").attr("disabled", true).addClass("disabled");
        $(".bank-account-form .button").attr("disabled", true).addClass("disabled");
    }
};

$(document).ready(function () {
    bankAccountForm.init();
});