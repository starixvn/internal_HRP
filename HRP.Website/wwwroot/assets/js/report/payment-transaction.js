var paymentTransaction = {
    rijndaelKey: '',
    rijndaelIV: '',

    init: function () {
        this.initSelect2();
        this.initDatetimepicker();
        this.initPaymentTransactionFanexMultiSelect();
        this.initEvents();
        this.rijndaelIV = $('#RijndaelIV').val();
        this.rijndaelKey = $('#RijndaelKey').val();
        this.decryptData();

        var $validations = $('#validation-summary');

        if ($('.field-validation-error', $validations).length > 0) {
            $validations.removeClass('hidden');
        } else {
            $validations.addClass('hidden');
        }

        commonFunctions.showHideValidationSummary($validations[0]);
    },

    decryptData: function () {
        if ($('#EncryptedPassword').val() !== '') {
            var data = rijndaelService.decryptData($('#EncryptedPassword').val(), this.rijndaelKey, this.rijndaelIV);
            $('#FilePassword').val(data);
        }
    },

    encryptData: function () {
        if ($('#FilePassword').val() !== '') {
            var data = rijndaelService.encryptData($('#FilePassword').val(), this.rijndaelKey, this.rijndaelIV);
            $('#EncryptedPassword').val(data);
        }
    },

    initEvents: function () {
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

        var that = this;

        $('#btn-export').on('click', function () {
            var $form = $('form');
            if ($form.valid()) {
                var $filePassword = $('#FilePassword');
                that.encryptData();
                $filePassword.attr('disabled', 'disabled');
                $form.submit();
                $filePassword.removeAttr('disabled');
            }
        });
    },

    initSelect2: function () {
        $(".js-select2").select2({
            minimumResultsForSearch: Infinity
        });
    },

    initDatetimepicker: function () {
        $(".datetimepicker").datetimepicker({
            viewMode: "months",
            format: "MM-YYYY"
        });

        $(".icon-calendar:not(.disabled)").on("click",
            function () {
                $(this).siblings(".datetimepicker").data("DateTimePicker").show();
            });
    },

    initPaymentTransactionFanexMultiSelect: function () {
        $.ajax({
            url: $("#get-payment-transactions-url").attr("href"),
            success: function (result) {
                $("#payment-transactions-multiselect").fanexMultiSelect({
                    listData: result,
                    useCustomCheckAllText: true,
                    customCheckAllText: "All",
                    showFilter: true,
                    onCheck: function () {
                        var selectedValue =
                            $("#payment-transactions-multiselect").fanexMultiSelect("getItems", true).join(",");
                        $("#selected-payment-transactions").val(selectedValue);
                    }
                });

                paymentTransaction.disableEnterOnFanexMultiSelect();

                var selectedValue =
                    $("#payment-transactions-multiselect").fanexMultiSelect("getItems", true).join(",");
                $("#selected-payment-transactions").val(selectedValue);
            }
        });
    },

    disableEnterOnFanexMultiSelect: function () {
        $("#fanex-multiselect-filter-input").off("keydown");

        $("#fanex-multiselect-filter-input").on("keydown",
            function (event) {
                if (event.keyCode === 13) {
                    event.preventDefault();
                    return false;
                } else {
                    return true;
                }
            });
    }
}

$(document).ready(function () {
    paymentTransaction.init();
});