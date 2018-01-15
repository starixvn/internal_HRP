var multiselect = {
    init: function () {
        var selectElements = $("select");
        selectElements.each(function(){
            var options = multiselect.getItemsList($(this));
            $(this).fanexMultiSelect({
                listData: options
            });
        });

        // $("select").fanexMultiSelect({
        //     listData: [
        //         {
        //             id: "Food",
        //             value: "Food",
        //             text: "Food",
        //             checked: true,
        //             enable: true,
        //             cssClass: "demo-class"
        //         }, {
        //             id: "Food",
        //             value: "Food",
        //             text: "Food",
        //             checked: true,
        //             enable: true,
        //             cssClass: "demo-class"
        //         }]
        // });
    },

    getItemsList: function (selectElement) {
        if ($(selectElement).is("select")) {
            if ($(selectElement).prop('multiple')) {
                // select element with multiple prop
                
            }
        }
    }
};

$(document).ready(function () {
    multiselect.init();
});