var navs = {
    init: function () {
        var activeTab = "#" + $(".nav-tabs").data("active");
        $(".nav-tabs a[href='" +activeTab + "']").tab("show");
    }
};

$(document).ready(navs.init);