var dynamicReport = (function ($) {
    var $selectedIds, $employeeStatusIds, $employeeStatusMultiSelect;

    function moveUpDown(up) {
        var $selectedItems = $('#selected-list option:selected');

        if ($selectedItems.length === 0)
            return;

        if (up) {
            $selectedItems.first().prev().before($selectedItems);
        } else {
            $selectedItems.last().next().after($selectedItems);
        }
    }

    function getElements() {
        $selectedIds = $('#SelectedIds');
        $employeeStatusIds = $("#EmployeeStatusIds");
        $employeeStatusMultiSelect = $("#employee-status");
    }

    function initSelect2() {
        $('.select2').select2({
            minimumResultsForSearch: Infinity
        });
    }

    function initFanexMultiSelect() {
        $employeeStatusMultiSelect.fanexMultiSelect({
            listData: JSON.parse($('#EmployeeStatuses').val()),
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            onCheck: function () {
                debugger;
                var selectedValue = $employeeStatusMultiSelect.fanexMultiSelect('getItems', true).join(',');
                $employeeStatusIds.val(selectedValue);
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
            $('#select-list option').prop('selected', false).detach().appendTo('#selected-list');
        });

        $('#btn-deselect-all').on('click', function () {
            var $selectedItems = $('#selected-list option');
            $selectedItems.detach();
            $selectedItems.each(function (i, element) {
                var $element = $(element);
                $element.prop('selected', false).appendTo($element.data('parent-id'));
            });
        });

        $('#dynamic-report-form').submit(function () {
            var $options = $('#selected-list option');
            var selectedIds = [];

            $options.each(function (i, element) {
                selectedIds.push(element.value);
            });

            $selectedIds.val(selectedIds.toString());
            alert(selectedIds.toString());
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
    dynamicReport.init();
});