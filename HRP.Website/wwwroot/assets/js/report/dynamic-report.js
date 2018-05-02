var dynamicReport = (function ($) {
    var $selectedIds;

    function moveUpDown(up) {
        var $selectedItems = $('#selected-list option:selected');

        if ($selectedItems.length == 0)
            return;

        if (up) {
            $selectedItems.first().prev().before($selectedItems);
        } else if (btnDown) {
            $selectedItems.last().next().after($selectedItems);
        }
    }

    function initEvents() {
        $('#btn-select').on('click', function () {
            var $selectedItems = $('#select-list option:selected');
            $selectedItems.detach();
            $selectedItems.appendTo('#selected-list')
        });

        $('#btn-deselect').on('click', function () {
            var $selectedItems = $('#selected-list option:selected');
            $selectedItems.detach();
            $selectedItems.each(function (i, element) {
                var $element = $(element);
                $element.appendTo($element.data('parent'));
            });
        });

        $('#btn-select-all').on('click', function () {
            $('#select-list .item').detach().appendTo('#selected-list');
        });

        $('#btn-deselect-all').on('click', function () {
            var $selectedItems = $('#selected-list .item');
            $selectedItems.detach();
            $selectedItems.each(function (i, element) {
                var $element = $(element);
                $element.appendTo($element.data('parent'));
            });
        });

        $('#myform').submit(function () {
            var selectedIds = [];
            $('#selected-list .item').each(function (i, element) {
                var $element = $(element);
                selectedIds.push($element.data('value'));
            });

            $selectedIds.val(selectedIds.toString());
        });

        $('#btn-moveup').on('click', function () {
            moveUpDown(true);
        });

        $('#btn-movedown').on('click', function () {
            moveUpDown(false);
        });
    }

    function init() {
        initEvents();
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    monthlyPayroll.init();
});