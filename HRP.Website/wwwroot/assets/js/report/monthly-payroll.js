var monthlyPayroll = (function ($) {
    var $form, $password, $encryptedPassword, rijndaelIV, rijndaelKey;

    function getElements() {
        $form = $('#filterForm');
        $password = $('#FilePassword');
        $encryptedPassword = $('#EncryptedPassword');
    }

    function initEvents() {
        $('#btn-export', $form).on('click', function () {
            if ($form.valid()) {
                encryptData();
                $password.attr('disabled', 'disabled');
                $form.submit();
                $password.removeAttr('disabled');
            }
        });

        $('.pass-wrap').on('click', '.icon-view', function () {
            var $this = $(this);
            var $input = $this.next();
            $input.attr('type', 'text');
            $this.removeClass('icon-view').addClass('icon-hide');
        });

        $('.pass-wrap').on('click', '.icon-hide', function () {
            var $this = $(this);
            var $input = $this.next();
            $input.attr('type', 'password');
            $this.addClass('icon-view').removeClass('icon-hide');
        });

        commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
    }

    function initExtensions() {
        var maxdate = $('#SelectedTime').data('maxdate');
        $('.datetimepicker').datetimepicker({
            format: 'MM-YYYY',
            maxDate: maxdate,
            useCurrent: false
        });

        $('.select2').select2({
            minimumResultsForSearch: Infinity
        });

        $('.icon-calendar').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').toggle();
        });
    }

    function decryptData() {
        if ($encryptedPassword.val() !== '') {
            var data = rijndaelService.decryptData($encryptedPassword.val(), rijndaelKey, rijndaelIV);
            $password.val(data);
        }
    }

    function encryptData() {
        if ($password.val() !== '') {
            var data = rijndaelService.encryptData($password.val(), rijndaelKey, rijndaelIV);
            $encryptedPassword.val(data);
        }
    }

    function init() {
        rijndaelIV = $('#RijndaelIV').val();
        rijndaelKey = $('#RijndaelKey').val();

        getElements();
        decryptData();
        initExtensions();
        initEvents();
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    monthlyPayroll.init();
});