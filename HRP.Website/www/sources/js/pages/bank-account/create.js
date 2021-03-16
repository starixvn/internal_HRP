import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';
import './bank-account-form';
import './transform-select';

const createBankAccount = {
    init: function () {
        $(".back").on("click",
            function (e) {
                e.preventDefault();
                if (!$(this).hasClass("disabled")) {
                    window.history.back();
                }
            });
    }
}

$(document).ready(function () {
    createBankAccount.init();
});