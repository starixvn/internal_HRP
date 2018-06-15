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
        if (encrypt.encryptionKey !== "" && encrypt.encryptionIV !== "") {
            var encryptInputs = $("input.encrypt");

            encryptInputs.each(function () {
                var valueToDecrypt = $(this).val();
                var isEncrypted = $(this).data("isencrypted");

                if (valueToDecrypt === "undefined" || valueToDecrypt === "" || isEncrypted === "false") {
                    return true;
                }

                var decrypted = rijndaelService.decryptData(
                    valueToDecrypt,
                    encrypt.encryptionKey,
                    encrypt.encryptionIV
                );

                $(this).val(decrypted);
                $(this).data("isencrypted", false);

                return true;
            });
        }
    },

    encryptAndReplace: function () {
        $("form").on("submit", function () {
            if (encrypt.encryptionKey === "" || encrypt.encryptionIV === "") {
                return;
            }

            var form = $("form");
            form.validate();
            if (form.valid()) {
                var encryptInputs = $("input.encrypt");

                encryptInputs.each(function () {
                    var valueToEncrypt = $(this).val();
                    var isEncrypted = $(this).data("isencrypted");

                    // validator.element($(this));
                    if (valueToEncrypt === "undefined" || valueToEncrypt === "" || isEncrypted) {
                        return true;
                    }

                    var encrypted = rijndaelService.encryptData(
                        valueToEncrypt,
                        encrypt.encryptionKey,
                        encrypt.encryptionIV
                    );

                    $(this).val(encrypted);
                    $(this).data("isencrypted", true);

                    return true;
                });
            }
        });
    }
};

$(document).ready(function () {
    encrypt.init();
});