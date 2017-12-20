var navs = {
    init: function () {
        var activeTab = "#" + $(".nav-tabs").data("active");
        $(".nav-tabs a[href='" + activeTab + "']").tab("show");
    }
};

$(document).ready(function () {
    navs.init();

    $('.goback').click(function () {
        if (history.length === 1) {
            window.location = "/"
        } else {
            history.back();
        }
    })
});