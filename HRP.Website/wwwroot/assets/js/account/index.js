var navs = {
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