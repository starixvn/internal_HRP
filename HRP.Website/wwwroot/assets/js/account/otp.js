//$(document).ready(function () {
//    //$('.nav-tabs a[href="#@Model.ActivePanel"]').tab('show');

//    var qr = new QRious({
//        element: document.getElementById('setup-otp-qrcode'),
//        padding: 30,
//        value: '@Html.Raw(Model.SetupOtpViewModel.QrCodeText)',
//        size: 300
//    });

//    $("#verify-setup-otp-form").on("submit",
//        function (e) {
//            e.preventDefault();
//            $.ajax({
//                url: $(this).attr('action'),
//                method: $(this).attr('method'),
//                data: $(this).serialize(),
//                success: function (res, status, xhr) {
//                    $("#verify-setup-otp-fail").hide();
//                    $("#verify-setup-otp-success").show();

//                    $(location).attr("href", '/Account/VerifyOTP').delay(3000);
//                },
//                error: function (request, error) {
//                    $("#verify-setup-otp-fail").show();
//                    $("#verify-setup-otp-success").hide();
//                }
//            });
//            return false;
//        });
//});

var qrcode = {
    init: function() {
        var qrcodetext = $("#qrcodetext").data("qrcodetext");

        var qr = new QRious({
            element: document.getElementById('setup-otp-qrcode'),
            padding: 30,
            value: qrcodetext,
            size: 300
        });

        qrcode.setup();
    },

    setup: function() {
        $("#verify-setup-otp-form").on("submit",
            function(e) {
                e.preventDefault();
                $.ajax({
                    url: $(this).attr('action'),
                    method: $(this).attr('method'),
                    data: $(this).serialize(),
                    success: function(res, status, xhr) {
                        qrcode.otpSuccess();
                    },
                    error: function(request, error) {
                        qrcode.otpFail();
                    }
                });
                return false;
            });
    },

    otpFail: function() {
        $("#verify-setup-otp-fail").show();
        $("#verify-setup-otp-success").hide();
    },

    otpSuccess: function() {
        $("#verify-setup-otp-fail").hide();
        $("#verify-setup-otp-success").show();

        $(location).attr("href", '/Account/VerifyOTP').delay(3000);
    }
};

$(document).ready(qrcode.init);