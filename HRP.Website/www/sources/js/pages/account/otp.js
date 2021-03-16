import QRious from 'qrious';

const qrcode = {
    init: function () {
        qrcode.hideOtpStatus();

        var qrcodetext = $("#qrcodetext").data("qrcodetext");

        var qr = new QRious({
            element: document.getElementById("setup-otp-qrcode"),
            padding: null,
            value: qrcodetext,
            size: 185
        });

        qrcode.setup();
    },

    setup: function () {
        $("#verify-setup-otp-form").on("submit",
            function (e) {
                e.preventDefault();
                var data = $(this).serialize();
                $.ajax({
                    url: $(this).attr('action'),
                    method: $(this).attr('method'),
                    data: data,
                    success: function (res, status, xhr) {
                        qrcode.otpSuccess();
                    },
                    error: function (request, error) {
                        qrcode.otpFail();
                        var $otp = $("#verify-setup-otp-form").find('input[name=Otp]');
                        $otp.val("");
                        $otp.data("isencrypted", false);
                    }
                });
                return false;
            });
    },

    hideOtpStatus: function () {
        $("#verify-setup-otp-fail").hide();
        $("#verify-setup-otp-success").hide();
    },

    otpFail: function () {
        $("#verify-setup-otp-fail").show();
        $("#verify-setup-otp-success").hide();
    },

    otpSuccess: function () {
        $("#verify-setup-otp-fail").hide();
        $("#verify-setup-otp-success").show();

        $("#verify-setup-otp-form > fieldset").prop("disabled", true);

        window.setTimeout(qrcode.navigateToVerifyOTP, 3000);
    },

    navigateToVerifyOTP: function () {
        $(location).attr("href", "/Account/VerifyOTP");
    }
};

$(document).ready(function () {
    qrcode.init();
});