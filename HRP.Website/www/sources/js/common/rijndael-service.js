var rijndaelService = (function ($) {
    function encryptData(value, rijndaelKey, rijndaelIV) {
        var cipherText = CryptoJS.AES.encrypt(
            value,
            CryptoJS.enc.Utf8.parse(rijndaelKey),
            {
                iv: CryptoJS.enc.Utf8.parse(rijndaelIV),
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

        return cipherText.toString();
    }

    function decryptData(value, rijndaelKey, rijndaelIV) {
        var plainText = CryptoJS.AES.decrypt(
            value,
            CryptoJS.enc.Utf8.parse(rijndaelKey),
            {
                iv: CryptoJS.enc.Utf8.parse(rijndaelIV),
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

        return plainText.toString(CryptoJS.enc.Utf8);
    }

    return {
        encryptData: encryptData,
        decryptData: decryptData
    };
}(jQuery));