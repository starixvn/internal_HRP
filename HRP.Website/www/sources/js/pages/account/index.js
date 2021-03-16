import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';
import 'bootstrap/dist/js/bootstrap';
import '../../common/client-encryption';
import './update-profile';
import './change-password';
import './otp';

const navs = {
    init: function () {
        navs.showActiveTab();
        navs.handleGoback();
    },

    showActiveTab: function(){
        var activeTab = "#" + $(".nav-tabs").data("active");
        $(".nav-tabs a[href='" + activeTab + "']").tab("show");
    },

    handleGoback: function(){
        $('.goback').click(function() {
            if (history.length === 1) {
                window.location = "/";
            } else {
                history.back();
            }
        });
    }
};

$(document).ready(function() {
    navs.init();
});