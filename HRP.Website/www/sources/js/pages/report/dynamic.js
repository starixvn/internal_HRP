import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'select2/dist/js/select2';
import '../../../../../wwwroot/lib/fanex-multiselect/fanex.multiSelect';
import rijndaelService from '../../common/rijndael-service';

const dynamicReport = (function ($, commonFunctions, rijndaelService) {
    let $selectedIds, $employeeStatusIds, $employeeStatusMultiSelect, $form, $password, $encryptedPassword, rijndaelIV, rijndaelKey;

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
        $form = $('#dynamic-report-form');
        $password = $('#FilePassword');
        $encryptedPassword = $('#EncryptedPassword');
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
                $element.prop('selected', false).appendTo($element.data('parent-id'));
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
        });

        $('#btn-moveup').on('click', function () {
            moveUpDown(true);
        });

        $('#btn-movedown').on('click', function () {
            moveUpDown(false);
        });

        $('#btn-export', $form).on('click', function () {
            if ($form.valid()) {
                encryptData();
                $password.attr('disabled', 'disabled');
                $form.submit();
                $password.removeAttr('disabled');
            }
        });

        $('.pass-wrap').on('click', '.icon-view', function () {
            var $this = $(this);
            var $input = $this.next();
            $input.attr('type', 'text');
            $this.removeClass('icon-view').addClass('icon-hide');
        });

        $('.pass-wrap').on('click', '.icon-hide', function () {
            var $this = $(this);
            var $input = $this.next();
            $input.attr('type', 'password');
            $this.addClass('icon-view').removeClass('icon-hide');
        });

        commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
    }

    function decryptData() {
        if ($encryptedPassword.val() !== '') {
            var data = rijndaelService.decryptData($encryptedPassword.val(), rijndaelKey, rijndaelIV);
            $password.val(data);
        }
    }

    function encryptData() {
        if ($password.val() !== '') {
            var data = rijndaelService.encryptData($password.val(), rijndaelKey, rijndaelIV);
            $encryptedPassword.val(data);
        }
    }

    function init() {
        rijndaelIV = $('#RijndaelIV').val();
        rijndaelKey = $('#RijndaelKey').val();

        getElements();
        initSelect2();
        decryptData();
        initFanexMultiSelect();
        initEvents();
    }

    return {
        init: init
    };
}(jQuery, commonFunctions, rijndaelService));

$(document).ready(function () {
    dynamicReport.init();
});