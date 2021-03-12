import rijndaelService from './rijndael-service';

const encrypt = {
    encryptionKey: '',
    encryptionIV: '',

    init: function () {
        encrypt.getKey();
        encrypt.encryptAndReplace();
        encrypt.decryptAndReplace();
    },

    getKey: function () {
        encrypt.encryptionKey = $('.encryption-key').val();
        encrypt.encryptionIV = $('.encryption-iv').val();
    },

    decryptAndReplace: function () {
        if (encrypt.encryptionKey !== '' && encrypt.encryptionIV !== '') {
            let encryptInputs = $('input.encrypt');
            encryptInputs.each(function () {
                const valueToDecrypt = $(this).val(),
                    isEncrypted = $(this).data('isencrypted');

                if (valueToDecrypt === 'undefined' || valueToDecrypt === '' || isEncrypted === 'false') {
                    return true;
                }

                const decrypted = rijndaelService.decryptData(
                    valueToDecrypt,
                    encrypt.encryptionKey,
                    encrypt.encryptionIV
                );

                $(this).val(decrypted);
                $(this).data('isencrypted', false);

                return true;
            });
        }
    },

    encryptAndReplace: function () {
        $('form').on('submit', function () {
            if (encrypt.encryptionKey === '' || encrypt.encryptionIV === '') {
                return;
            }

            let form = $('form');
            form.validate();
            if (form.valid()) {
                let encryptInputs = $('input.encrypt');
                encryptInputs.each(function () {
                    const valueToEncrypt = $(this).val(),
                        isEncrypted = $(this).data('isencrypted');

                    if (valueToEncrypt === 'undefined' || valueToEncrypt === '' || isEncrypted) {
                        return true;
                    }

                    const encrypted = rijndaelService.encryptData(
                        valueToEncrypt,
                        encrypt.encryptionKey,
                        encrypt.encryptionIV
                    );

                    $(this).val(encrypted);
                    $(this).data('isencrypted', true);

                    return true;
                });
            }
        });
    }
};

$(document).ready(function () {
    encrypt.init();
});