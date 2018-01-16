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

var userManagement = (function ($) {
    var init = function () {
        $('.data-table').dataTable({
            "paging": false,
            "columnDefs": [{
                "orderable": false,
                "targets": 0
            }]
        });
    };

    $('#quick-search').off('keyup').on('keyup', function () {
        $('.data-table').DataTable().search($(this).val()).draw();
    });

    return {
        onSearchSuccess: function (response, status) {
            init();
        },
        init: init
    };
}(jQuery));

userManagement.init();

$(document).ready(function () {
    multiselect.init();
});