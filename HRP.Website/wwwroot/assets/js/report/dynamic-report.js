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

    function getElements() {
        $selectedIds = $('#SelectedIds');
        $employeeStatuses = $("#EmployeeStatuses");
    }

    function initSelect2() {
        $('#CompanyId').select2({
            allowClear: true,
            minimumResultsForSearch: Infinity
        });
    }

    function initFanexMultiSelect() {
        $("#employee-status").fanexMultiSelect({
            listData: result,
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            onCheck: function () {
                var selectedValue = $(this).fanexMultiSelect('getItems', true).join(',');
                $employeeStatuses.val(selectedValue);
            }
        });
    }

    function initEvents() {
        $('#btn-select').on('click', function () {
            var $selectedItems = $('#select-list option:selected');
            $selectedItems.detach();
            $selectedItems.prop('selected', false).appendTo('#selected-list')
        });

        $('#btn-deselect').on('click', function () {
            var $selectedItems = $('#selected-list option:selected');
            $selectedItems.detach();
            $selectedItems.each(function (i, element) {
                var $element = $(element);
                $element.prop('selected', false).appendTo($element.data('parent'));
            });
        });

        $('#btn-select-all').on('click', function () {
            $('#select-list option').detach().appendTo('#selected-list');
        });

        $('#btn-deselect-all').on('click', function () {
            var $selectedItems = $('#selected-list option');
            $selectedItems.detach();
            $selectedItems.each(function (i, element) {
                var $element = $(element);
                $element.prop('selected', false).appendTo($element.data('parent-id'));
            });
        });

        $('#myform').submit(function () {
            var $options = $('#selected-list options');
            var selectedIds = [];

            $options.each(function (i, element) {
                selectedIds.push(element.value);
            });

            $selectedIds.val(selectedIds.toString());
            return false;
        });

        $('#btn-moveup').on('click', function () {
            moveUpDown(true);
        });

        $('#btn-movedown').on('click', function () {
            moveUpDown(false);
        });
    }

    function init() {
        getElements();
        initSelect2();
        initFanexMultiSelect();
        initEvents();
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    monthlyPayroll.init();
});