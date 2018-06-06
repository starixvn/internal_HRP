var encrypt = {

    encryptionKey: "",
    encryptionIV: "",

    init: function () {
        encrypt.getKey();
        encrypt.encryptAndReplace();
        encrypt.decryptAndReplace();
    },

    getKey: function () {
        encrypt.encryptionKey = $(".encryption-key").val();
        encrypt.encryptionIV = $(".encryption-iv").val();
    },

    decryptAndReplace: function () {
        var isEncrypted = $(".encryption-isencrypted").val();

        if (isEncrypted === "true" && encrypt.encryptionKey !== "" && encrypt.encryptionIV !== "") {
            var key = CryptoJS.enc.Utf8.parse(encrypt.encryptionKey);
            var iv = CryptoJS.enc.Utf8.parse(encrypt.encryptionIV);

            var encryptInputs = $("input.encrypt");

            encryptInputs.each(function (i, obj) {
                var valueToDecrypt = $(this).val();

                var decrypted = CryptoJS.AES.decrypt(valueToDecrypt, key, {
                    iv: iv
                });

                $(this).val(CryptoJS.enc.Utf8.stringify(decrypted));
            });

            $(".encryption-isencrypted").val(false);
        }
    },

    encryptAndReplace: function () {
        $("form").on("submit",
            function (e) {
                var isEncrypted = $(".encryption-isencrypted").val();
                if (encrypt.encryptionKey === "" || encrypt.encryptionIV === "" || isEncrypted === "true") {
                    return;
                }

                var validator = $(this).validate();

                var key = CryptoJS.enc.Utf8.parse(encrypt.encryptionKey);
                var iv = CryptoJS.enc.Utf8.parse(encrypt.encryptionIV);

                var encryptInputs = $("input.encrypt");

                encryptInputs.each(function (i, obj) {
                    var valueToEncrypt = $(this).val();

                    validator.element($(this));

                    var encrypted = CryptoJS.AES.encrypt(
                        CryptoJS.enc.Utf8.parse(valueToEncrypt),
                        key, {
                            iv: iv
                        });

                    $(this).val(encrypted);
                });

                $(".encryption-isencrypted").val(true);
            });
    }
}

$(document).ready(function () {
    encrypt.init();
});