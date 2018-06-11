'use strict';

var monthlyPayroll = (function ($) {
    var $btnBack, $table, companyId;

    function initSelect2() {
        $('.js-select2').select2({
            minimumResultsForSearch: Infinity,
            width: '92px'
        });
    }

    function initDataTable() {
        $('#monthly-payroll-table').dataTable({
            ordering: true,
            order: [],
            lengthMenu: [commonFunctions.pagerRanges, commonFunctions.pagerRanges],
            dom: 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            info: false,
            language: {
                lengthMenu: 'Page size _MENU_',
                search: '',
                paginate: {
                    first: 'First',
                    last: 'Last',
                    next: '<i class="icon-next"></i>',
                    previous: '<i class="icon-back"></i>'
                },
                zeroRecords: commonFunctions.noDataOnTableMsg,
                sZeroRecords: commonFunctions.noDataOnTableMsg
            },
            aoColumnDefs: [
                { className: 'col-width-58', orderable: true, targets: 0, sType: "sortByStatus", bSortable: true },
                { orderable: true, targets: 1 },
                { orderable: true, targets: 2 },
                { orderable: true, targets: 3 },
                { className: 'col-width-76', orderable: true, targets: 4 },
                { orderable: false, targets: 5 },
                { orderable: false, targets: 6 },
                { orderable: false, targets: 7 },
                { orderable: false, targets: 8 },
                { orderable: false, targets: 9 },
                { orderable: false, targets: 10 },
                { orderable: false, targets: 11 },
                { orderable: false, targets: 12 },
                { orderable: false, targets: 13 },
                { orderable: false, targets: 14 },
                { orderable: false, targets: 15 },
                { orderable: false, targets: 16 },
                { orderable: false, targets: 17 },
                { orderable: false, targets: 18 },
                { className: 'col-width-60', orderable: false, targets: 19 },
                { className: 'col-width-50', orderable: false, targets: 20 }
            ],
            scrollX: true,
            scrollY: '300px',
            scrollCollapse: true,
            responsive: true,
            fixedColumns: {
                leftColumns: 5,
                rightColumns: 2
            }
        });

        $(window).trigger('resize');

        $('select[name=monthly-payroll-table_length]').select2({
            minimumResultsForSearch: Infinity
        });
    }

    function searchEmployeeSuggestion() {
        $('#SearchEmployee').autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');
                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: commonFunctions.rootURL + 'PaymentProcess/GetEmployeeSuggestionByName',
                    dataType: 'json',
                    data: {
                        companyId: companyId,
                        name: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.nickname + ' (' + item.fullName + ')',
                                    value: item.employeeId
                                };
                            }));
                        }
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $('#SearchEmployee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
                $('#Employee').val(ui.item.label);

                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                return false;
            }
        });

        $('#SearchEmployee').keyup(function () {
            $('#Employee').val($(this).val());

            if (!$(this).val()) {
                $('#EmployeeId').val('');
            }
        });

        $('#SearchIDNo').autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');
                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: commonFunctions.rootURL + 'PaymentProcess/GetEmployeeSuggestionByIDNo',
                    dataType: 'json',
                    data: {
                        companyId: companyId,
                        idNo: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.idNo,
                                    value: item.idNo
                                };
                            }));
                        }
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $('#SearchIDNo').val(ui.item.label);
                $('#IDNo').val(ui.item.label);

                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                return false;
            }
        });

        $('#SearchIDNo').keyup(function () {
            $('#IDNo').val($(this).val());
        });
    }

    function initContextMenu() {
        $.contextMenu({
            selector: '.js-contextMenu',
            trigger: 'left',
            build: function ($trigger, e) {
                var $element = $(e.currentTarget);
                var basicSalary = {
                    title: $element.next().find('.js-menuItem-basicSalary').html(),
                    url: $element.next().find('.js-menuItem-basicSalary').attr('href'),
                    hasSetting: $element.next().find('.js-menuItem-basicSalary').data('hassetting') === 'True'
                };
                var allowance = {
                    title: $element.next().find('.js-menuItem-allowance').html(),
                    url: $element.next().find('.js-menuItem-allowance').attr('href'),
                    hasSetting: $element.next().find('.js-menuItem-allowance').data('hassetting') === 'True'
                };
                var presetMonthlyPayroll = $element.next().find('.js-menuItem-presetMonthlyPayroll');
                var modalHasNoPayrollSetting = $('#modalHasNoPayrollSetting');
                var items = {};

                if (basicSalary.title) {
                    items = {
                        'BasicSalary': { name: basicSalary.title, isHtmlName: true },
                        'Allowances': { name: allowance.title, isHtmlName: true }
                    };

                    if (presetMonthlyPayroll.length > 0) {
                        items = {
                            'BasicSalary': { name: basicSalary.title, isHtmlName: true },
                            'Allowances': { name: allowance.title, isHtmlName: true },
                            'PresetMonthlyPayroll': { name: presetMonthlyPayroll.html(), isHtmlName: true }
                        };
                    }
                } else {
                    if (presetMonthlyPayroll.length > 0) {
                        items = {
                            'PresetMonthlyPayroll': { name: presetMonthlyPayroll.html(), isHtmlName: true }
                        };
                    }
                }

                return {
                    callback: function (key, options) {
                        switch (key) {
                            case 'BasicSalary':
                                if (basicSalary.hasSetting) {
                                    location.href = basicSalary.url;
                                } else {
                                    modalHasNoPayrollSetting.find('.no-basic-salary-setting').removeClass('hidden');
                                    modalHasNoPayrollSetting.find('.no-allowance-setting').addClass('hidden');
                                    modalHasNoPayrollSetting.modal('show');
                                }
                                break;

                            case 'Allowances':
                                if (allowance.hasSetting) {
                                    location.href = allowance.url;
                                } else {
                                    modalHasNoPayrollSetting.find('.no-basic-salary-setting').addClass('hidden');
                                    modalHasNoPayrollSetting.find('.no-allowance-setting').removeClass('hidden');
                                    modalHasNoPayrollSetting.modal('show');
                                }
                                break;

                            case 'PresetMonthlyPayroll':
                                $('#modalPresetPayroll input[name=employeeId]').val(presetMonthlyPayroll.data('employeeid'));
                                $('#modalPresetPayroll').modal('show');
                                break;
                        }
                    },
                    items: items
                };
            }
        });
    }

    function initCostCenterMultiSelect() {
        $.ajax({
            url: commonFunctions.rootURL + 'PaymentProcess/GetHierarchyCompany',
            dataType: 'json',
            data: {
                companyId: $('#CompanyId').val(),
                costCenterIds: $("#CostCenterIds").val()
            },
            success: function (result) {
                $("#CostCenterMultiSelect").fanexMultiSelect({
                    listData: result,
                    useCustomCheckAllText: true,
                    customCheckAllText: "All",
                    onCheck: function () {
                        var costCenterIds = $("#CostCenterMultiSelect").fanexMultiSelect('getItems', true).join(',');
                        $("#CostCenterIds").val(costCenterIds);
                    }
                });

                var costCenterIds = $("#CostCenterMultiSelect").fanexMultiSelect('getItems', true).join(',');
                $("#CostCenterIds").val(costCenterIds);
            }
        });
    }

    function registerEventSubmitForm() {
        $('form.search-form').on('submit', function (e) {
            var costCenterIds = $("#CostCenterIds").val();

            if (!costCenterIds) {
                $('.list-message-error').removeClass('hidden');
                $('#CostCenterMultiSelect').addClass('validation-error');

                e.preventDefault();
            }
        });
    }

    function decryptData() {
        var rijndaelKey = $('#RijndaelKey').val();
        var rijndaelIV = $('#RijndaelIV').val();

        $table.DataTable().$('.js-encrypted').each(function () {
            var value = $(this).html();
            var plainText = rijndaelService.decryptData(value, rijndaelKey, rijndaelIV);

            if (parseFloat(plainText) < 0) {
                $(this).addClass('negative-amount');
            }

            $(this).html(plainText).removeClass('hidden');
        });
    }

    function initModule() {
        companyId = parseInt($('#CompanyId').val(), 10);
        $btnBack = $('.js-btnBack');
        $table = $('#monthly-payroll-table');
        initSelect2();
        searchEmployeeSuggestion();
        initContextMenu();
        registerEventSubmitForm();

        decryptData();

        $('#quick-search').off('keyup').on('keyup', function () {
            $table.DataTable().search($(this).val()).draw();
        });
    }

    return {
        initModule: initModule,
        initDataTable: initDataTable,
        initCostCenterMultiSelect: initCostCenterMultiSelect,
        onPresetSuccess: function (response) {
            if (response.isSuccessful) {
                $('form.search-form').submit();
            } else {
                $('#modalPresetPayroll').modal('hide');
                commonFunctions.showErrorMessage($('.msg-error'), response.message);
            }
        },
        onPresetAllSuccess: function (response) {
            if (response.isSuccessful) {
                $('form.search-form').submit();
            } else {
                $('#modalPresetAllPayroll').modal('hide');
                commonFunctions.showErrorMessage($('.msg-error'), response.message);
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    monthlyPayroll.initModule();

    monthlyPayroll.initCostCenterMultiSelect();
});

if (document.readyState === 'loading') {
    monthlyPayroll.initDataTable();
}