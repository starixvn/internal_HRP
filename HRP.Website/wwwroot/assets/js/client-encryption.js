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

    encryptData: function(value, rijndaelKey, rijndaelIV) {
        return CryptoJS.AES.encrypt(
            value,
            rijndaelKey, {
                iv: rijndaelIV,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });
    },

    decryptData: function(value, rijndaelKey, rijndaelIV) {
        return CryptoJS.AES.decrypt(value, rijndaelKey, {
            iv: rijndaelIV,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    },

    decryptAndReplace: function () {
        if (encrypt.encryptionKey !== "" && encrypt.encryptionIV !== "") {
            var key = CryptoJS.enc.Utf8.parse(encrypt.encryptionKey);
            var iv = CryptoJS.enc.Utf8.parse(encrypt.encryptionIV);

            var encryptInputs = $("input.encrypt");

            encryptInputs.each(function (i, obj) {
                var valueToDecrypt = $(this).val();
                var isEncrypted = $(this).data("isencrypted");

                if (valueToDecrypt === "undefined" || valueToDecrypt === "" || isEncrypted === "false") {
                    return true;
                }

                var decrypted = encrypt.decryptData(valueToDecrypt, key, iv);

                $(this).val(CryptoJS.enc.Utf8.stringify(decrypted));
                $(this).data("isencrypted", false);
            });
        }
    },

    encryptAndReplace: function () {
        $("form").on("submit",
            function (e) {
                if (encrypt.encryptionKey === "" || encrypt.encryptionIV === "") {
                    return;
                }
                var validator = $("form").validate();

                var key = CryptoJS.enc.Utf8.parse(encrypt.encryptionKey);
                var iv = CryptoJS.enc.Utf8.parse(encrypt.encryptionIV);

                var encryptInputs = $("input.encrypt");

                encryptInputs.each(function (i, obj) {
                    var valueToEncrypt = $(this).val();
                    var isEncrypted = $(this).data("isencrypted");

                    validator.element($(this));
                    if (valueToEncrypt === "undefined" || valueToEncrypt === "" || isEncrypted) {
                        return true;
                    }

                    var encrypted = encrypt.encryptData(valueToEncrypt, key, iv);

                    $(this).val(encrypted);
                    $(this).data("isencrypted", true);
                });
            });
    },
}

$(document).ready(function () {
    encrypt.init();
});