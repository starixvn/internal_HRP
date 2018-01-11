'use strict';

var updateEmployment = (function ($) {
    var $employmentHeaderTab, $employmentTab, $updateEmploymentForm, $currentDate,
        $bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement,
        defaultOption = '<option value="" style="display: none;">--Please select--</option>';

    function registerEventChangeBVICompany() {
        $bviCompanyElement.on('change', function () {
            var companyId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetDivisionByCompanyId',
                dataType: 'json',
                data: {
                    companyId: companyId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].divisionId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $divisionElement.html(htmlOptions);
                    $divisionElement.select2('destroy').select2();
                    // reset
                    $departmentElement.html(defaultOption);
                    $sectionElement.html(defaultOption);
                    $costCenterElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeDivision() {
        $divisionElement.on('change', function () {
            var divisionId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetDepartmentByDivisionId',
                dataType: 'json',
                data: {
                    divisionId: divisionId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].departmentId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $departmentElement.html(htmlOptions);
                    $departmentElement.select2('destroy').select2();
                    // reset
                    $sectionElement.html(defaultOption);
                    $costCenterElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeDepartment() {
        $departmentElement.on('change', function () {
            var departmentId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetSectionByDepartmentId',
                dataType: 'json',
                data: {
                    departmentId: departmentId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].sectionId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $sectionElement.html(htmlOptions);
                    $sectionElement.select2('destroy').select2();
                    // reset
                    $costCenterElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeSection() {
        $sectionElement.on('change', function () {
            var sectionId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetCostCenterBySectionId',
                dataType: 'json',
                data: {
                    sectionId: sectionId
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        var totalItems = data.length;
                        htmlOptions = defaultOption;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].costCenterId + '">' + data[i].code + '</option>';
                        }
                    } else {
                        htmlOptions = defaultOption;
                    }

                    $costCenterElement.html(htmlOptions);
                    $costCenterElement.select2('destroy').select2();
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function initSelect2() {
        $updateEmploymentForm.find('select.search-suggestion').select2();
    }

    function calculateLengthOfService() {
        $currentDate = $('#CurrentDate');

        $('#GroupStartDate').on('dp.change', function (e) {
            var starts = moment($currentDate.val()),
                ends = e.date,
                intervals = ['years', 'months', 'days'],
                out = [];

            for (var i = 0; i < intervals.length; i++) {
                var diff = starts.diff(ends, intervals[i]);
                ends.add(diff, intervals[i]);
                out.push(diff + ' ' + intervals[i].replace('s', '(s)'));
            }

            $('#ServiceGroup').val(out.join('/'));
        });

        $('#CompanyStartDate').on('dp.change', function (e) {
            var starts = moment($currentDate.val()),
                ends = e.date,
                intervals = ['years', 'months', 'days'],
                out = [];

            for (var i = 0; i < intervals.length; i++) {
                var diff = starts.diff(ends, intervals[i]);
                ends.add(diff, intervals[i]);
                out.push(diff + ' ' + intervals[i].replace('s', '(s)'));
            }

            $('#ServiceCompany').val(out.join('/'));
        });
    }

    function initSearchAutocomplete($selectorName, $selectorFullName, $selectorNickName) {
        $selectorName.autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');

                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: commonFunctions.rootURL + 'EmployeeManagement/GetEmployeeSuggestion',
                    dataType: 'json',
                    data: {
                        idNo: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.nickName + ' (' + item.fullName + ')',
                                    fullname: item.fullName,
                                    nickname: item.nickName,
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
                $selectorFullName.val(ui.item.fullname);
                $selectorNickName.val(ui.item.nickname);

                return false;
            }
        });
    }

    function initDatepicker() {
        $updateEmploymentForm.find('input.datetimepicker').each(function () {
            var $this = $(this),
                defaultDate = $this.val(),
                mindate = $this.data('mindate'),
                maxdate = $this.data('maxdate');

            $this.datetimepicker({
                format: 'MMM DD, YYYY',
                minDate: mindate,
                maxDate: maxdate
            });

            if (defaultDate) {
                defaultDate = moment(defaultDate).format('MMM DD, YYYY')
                $this.data("DateTimePicker").date(defaultDate);
            } else {
                $this.val('');
            }
        });

        $updateEmploymentForm.find('.icon-calendar').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').show();
        });
    }

    function reparseform() {
        $updateEmploymentForm.removeData('validator').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse('#updateEmploymentForm');

        var $validator = $('#createEmploymentForm').validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);

            if ($element.hasClass('search-suggestion')) {
                $element.next().find('.select2-selection').addClass(errorClass).addClass('control').removeClass(validClass);
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('search-suggestion')) {
                $element.next().find('.select2-selection').removeClass(errorClass).removeClass('control').addClass(validClass);
            }
        };
    }

    function loadEmploymentForm() {
        $employmentHeaderTab.on('click', function () {
            var $this = $(this);
            var isFirstLoad = $this.data('isfirstload');

            if (isFirstLoad) {
                $this.data('isfirstload', false);

                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'EmployeeManagement/UpdateEmployment',
                    dataType: 'html',
                    data: {
                        employeeId: 1
                    },
                    success: function (data) {
                        $employmentTab.html(data);

                        $updateEmploymentForm = $('#updateEmploymentForm');
                        $bviCompanyElement = $('#CompanyId');
                        $divisionElement = $('#DivisionId');
                        $departmentElement = $('#DepartmentId');
                        $sectionElement = $('#SectionId');
                        $costCenterElement = $('#CostCenterId');

                        reparseform();
                        initDatepicker();
                        initSelect2();
                        calculateLengthOfService();

                        registerEventChangeBVICompany();
                        registerEventChangeDivision();
                        registerEventChangeDepartment();
                        registerEventChangeSection();
                        initSearchAutocomplete($('#NoneOrIDNoPFileAdmin'), $('#FullNamePFileAdmin'), $('#NickNamePFileAdmin'));
                        initSearchAutocomplete($('#NoneOrIDNoBVISalary'), $('#Paymaster_FullNameBVISalary'), $('#Paymaster_NickNameBVISalary'));
                        initSearchAutocomplete($('#NoneOrIDNoBVIAllowance'), $('#Paymaster_FullNameBVIAllowance'), $('#Paymaster_NickNameBVIAllowance'));
                        initSearchAutocomplete($('#NoneOrIDNoLocalSalary'), $('#Paymaster_FullNameLocalSalary'), $('#Paymaster_NickNameLocalSalary'));
                        initSearchAutocomplete($('#NoneOrIDNoAdministrator'), $('#EPA_FullNameAdministrator'), $('#EPA_NickNameAdministrator'));
                        initSearchAutocomplete($('#NoneOrIDNoEvaluator1'), $('#EPA_FullNameEvaluator1'), $('#EPA_NickNameEvaluator1'));
                        initSearchAutocomplete($('#NoneOrIDNoEvaluator2'), $('#EPA_FullNameEvaluator2'), $('#EPA_NickNameEvaluator2'));
                        initSearchAutocomplete($('#NoneOrIDNoReviewer'), $('#EPA_FullNameReviewer'), $('#EPA_NickNameReviewer'));
                        initSearchAutocomplete($('#NoneOrIDNoManagement'), $('#EPA_FullNameManagement'), $('#EPA_NickNameManagement'));
                        initSearchAutocomplete($('#NoneOrIDNoIncrement'), $('#EPA_FullNameIncrement'), $('#EPA_NickNameIncrement'));
                        initSearchAutocomplete($('#NoneOrIDNoBVIBonus'), $('#EPA_FullNameBVIBonus'), $('#EPA_NickNameBVIBonus'));
                        initSearchAutocomplete($('#NoneOrIDNoLocalBonus'), $('#EPA_FullNameLocalBonus'), $('#EPA_NickNameLocalBonus'));
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function initModule() {
        $employmentTab = $('#tab-employment');
        $employmentHeaderTab = $('#tab-header-employment');

        loadEmploymentForm();
    }

    return {
        InitModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                $('.validation-summary-errors').addClass('hidden');
                $('.validation-summary-errors ul').html('');
            } else {
                var htmlErrors = '';
                if (response.errors) {
                    response.errors.forEach(function (error) {
                        htmlErrors += '<li>' + error + '</li>';
                    });
                } else {
                    htmlErrors += '<li>' + response.message + '</li>';
                }

                $('.validation-summary-errors').removeClass('hidden');
                $('.validation-summary-errors ul').html(htmlErrors);
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    updateEmployment.InitModule();
});