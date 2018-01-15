var multiselect = {
    init: function () {
        // var selectElements = $("select");
        // selectElements.each(function(){
        //     var options = multiselect.getItemsList($(this));
        //     $(this).fanexMultiSelect({
        //         listData: options,
        //         useCustomCheckAllText: true,
        //         customCheckAllText: 'All'
        //     });
        // });

        $("select").ultraselect({
            selectAllText: "All"
        });
    },

    // getItemsList: function (selectElement) {
    //     if ($(selectElement).is("select")) {
    //         if ($(selectElement).prop('multiple')) {
    //             // select element with multiple prop
    //             var values = [];
    //             $.each($(selectElement).prop("options"), function(){
    //                 var item = {
    //                     id: $(selectElement).prop("name"),
    //                     value: this.value,
    //                     text: this.text,
    //                     checked: this.selected,
    //                     enable: true,
    //                     cssClass: "-option-check"
    //                 }
    //                 values.push(item);
    //             });

    //             return values;
    //         }
    //     }
    // }
};

$(document).ready(function () {
    multiselect.init();
});