var bankAccountForm = {
    init: function () {
        bankAccountForm.addRequiredIfRelatedFieldIsTrueClientValidation();
        bankAccountForm.toggleRequiredMarkOnBankTypeChanged();
        bankAccountForm.handleFormSubmittedInModal();
        bankAccountForm.removeBackdropOnModalHide();
        bankAccountForm.handleFormSuccessed();
        bankAccountForm.checkFormError();
        bankAccountForm.initPaymentTransactionSelect();
    },

    initPaymentTransactionSelect: function () {
        $(".bank-account-form").on("change",
            ".change-payment-transaction",
            function () {
                var ajaxUrl = $(".payment-transaction-select").data("url") +
                    "?bankTypeId=" +
                    $("#BankTypeId").val() +
                    "&employeeId=" +
                    $("#EmployeeId").val();

                $.ajax({
                    url: ajaxUrl,
                    method: "get",
                    success: function (results) {
                        $(".payment-transaction-select").select2().empty();
                        $(".payment-transaction-select").select2({
                            width: "100%",
                            data: results
                        });

                        var selectedPaymentTransactionId = $("#selected-payment-transaction-id").val();
                        if (selectedPaymentTransactionId !== "0") {
                            $("#PaymentTransactionId").val(selectedPaymentTransactionId).trigger("change");
                        }
                    }
                });
            });

        var ajaxUrl = $(".payment-transaction-select").data("url") +
            "?bankTypeId=" +
            $("#BankTypeId").val() +
            "&employeeId=" +
            $("#EmployeeId").val();

        $.ajax({
            url: ajaxUrl,
            method: "get",
            success: function (results) {
                $(".payment-transaction-select").select2().empty();
                $(".payment-transaction-select").select2({
                    width: "100%",
                    data: results
                });

                var selectedPaymentTransactionId = $("#selected-payment-transaction-id").val();
                if (selectedPaymentTransactionId !== "0") {
                    $("#PaymentTransactionId").val(selectedPaymentTransactionId).trigger("change");
                }
            }
        });
    },

    checkIfRequired: function (desiredvalue, relatedFieldName) {
        desiredvalue = (desiredvalue == null ? "" : desiredvalue).toString();
        var controlType = $("input[id$='" + relatedFieldName + "']").attr("type");
        var actualvalue;
        if (controlType === "checkbox" || controlType === "radio") {
            var control = $("input[id$='" + relatedFieldName + "']:checked");
            actualvalue = control.val();
        } else {
            actualvalue = $("#" + relatedFieldName).val();
        }

        return $.trim(desiredvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase();
    },

    toggleRequiredMarkOnBankTypeChanged: function () {
        $(".bank-account-form").on("change",
            "#BankTypeId",
            function () {
                $("[data-val-requiredifrelatedfieldistrue]").each(function () {
                    var desiredValue = $(this).data("val-requiredifrelatedfieldistrue-desiredvalue");
                    var relatedFieldName = $(this).data("val-requiredifrelatedfieldistrue-relatedfieldname");
                    var labelFor = $(this).attr("name");

                    if (bankAccountForm.checkIfRequired(desiredValue, relatedFieldName)) {
                        $("label[for='" + labelFor + "']").addClass("-has-required");
                    } else {
                        $("label[for='" + labelFor + "']").removeClass("-has-required");
                    }
                });
            });

        $("#BankTypeId").trigger("change");
    },

    addRequiredIfRelatedFieldIsTrueClientValidation: function () {
        $.validator.addMethod("requiredifrelatedfieldistrue",
            function (value, element, parameters) {
                var desiredvalue = parameters.desiredvalue;
                desiredvalue = (desiredvalue == null ? "" : desiredvalue).toString();
                var controlType = $("input[id$='" + parameters.relatedfieldname + "']").attr("type");
                var actualvalue;
                if (controlType === "checkbox" || controlType === "radio") {
                    var control = $("input[id$='" + parameters.relatedfieldname + "']:checked");
                    actualvalue = control.val();
                } else {
                    actualvalue = $("#" + parameters.relatedfieldname).val();
                }

                if ($.trim(desiredvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase()) {
                    var isValid = $.validator.methods.required.call(this, value, element, parameters);
                    return isValid;
                }

                return true;
            });

        $.validator.unobtrusive.adapters.add("requiredifrelatedfieldistrue",
            ["relatedfieldname", "desiredvalue"],
            function (options) {
                options.rules["requiredifrelatedfieldistrue"] = options.params;
                options.messages["requiredifrelatedfieldistrue"] = options.message;
            });

        bankAccountForm.reinitValidation($(".bank-account-form"));
    },

    reinitValidation: function ($form) {
        $form.removeData("validator").removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse($form);
        $form.validate();
    },

    removeBackdropOnModalHide: function () {
        $("#dynamic-modals").on("hidden.bs.modal",
            ".modal",
            function () {
                $(this).remove();
                $(".modal-backdrop").remove();
            });
    },

    resetFormClientValidation: function () {
        var form = $("#dynamic-modals form")
            .removeData("validator") /* added by the raw jquery.validate plugin */
            .removeData("unobtrusiveValidation"); /* added by the jquery unobtrusive plugin*/

        $.validator.unobtrusive.parse(form);
    },

    handleFormSubmittedInModal: function () {

        commonFunctions.disableEnterKeySubmit($(".bank-account-form"));

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

                            bankAccountForm.resetFormClientValidation();

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
                    var redirectLink = $("#redirect-link").prop("href");
                    if (redirectLink !== undefined) {
                        window.location.replace(redirectLink);
                    } else {
                        window.location.href = $("#index-link").prop("href");
                    }
                }
            },
                bankAccountForm.waitTime);
        }
    },

    waitTime: 5000,

    countTimeInSeconds: 5,

    checkFormError: function () {
        console.log("custom check form for error");

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

$.validator.setDefaults({
    highlight: function (element, errorClass, validClass) {
        if (element.type === "radio") {
            this.findByName(element.name).addClass(errorClass).removeClass(validClass);
        } else {
            $(element).addClass(errorClass).removeClass(validClass);
        }

        console.log("custom check form for error");

        if ($(".bank-account-form").find(".field-validation-error").length === 0) {
            // Valid
            $(".bank-account-form .list-message-error").addClass("hidden");
        } else {
            $(".bank-account-form .list-message-error").removeClass("hidden");
        }
    },
    unhighlight: function (element, errorClass, validClass) {
        if (element.type === "radio") {
            this.findByName(element.name).removeClass(errorClass).addClass(validClass);
        } else {
            $(element).removeClass(errorClass).addClass(validClass);
        }

        console.log("custom check form for error");

        if ($(".bank-account-form").find(".field-validation-error").length === 0) {
            // Valid
            $(".bank-account-form .list-message-error").addClass("hidden");
        } else {
            $(".bank-account-form .list-message-error").removeClass("hidden");
        }
    }
});

$(document).ready(function () {
    bankAccountForm.init();
});