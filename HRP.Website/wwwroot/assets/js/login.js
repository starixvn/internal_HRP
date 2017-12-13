$(document).ready(function () {
    $("#login").submit(function (event) {
        event.preventDefault();
        var allInputs = $("input").length;
        var filledInputs = 0;
        $('input').each(function (index, element) {
            var inputValue = $(element).val();
            if (!inputValue) {
                $(this).parent().effect('shake');
            } else {
                filledInputs++;
            }
        });

        if (allInputs === filledInputs) {
            var password = $("#Password").val();
            var key = CryptoJS.enc.Utf8.parse('8080808080808080');
            var iv = CryptoJS.enc.Utf8.parse('8080808080808080');
            var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(password), key,
                {
                    keySize: 128 / 8,
                    iv: iv,
                    mode: CryptoJS.mode.CBC,
                    padding: CryptoJS.pad.Pkcs7
                });
            $('#Password').val(encrypted);
            event.currentTarget.submit();
            $("#Password").val(password);
        }
    });
});