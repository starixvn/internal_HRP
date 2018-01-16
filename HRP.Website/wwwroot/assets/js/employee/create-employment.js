'use strict';

var createEmployment = (function ($) {
    var $createEmploymentForm, currentDate,
        $bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement,
        $groupStartDateElement, $groupEndDateElement, $companyStartDateElement, $companyEndDateElement,
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

    function initSelect2($form) {
        $form.find('select.search-suggestion').select2();
    }

    function calculateLengthOfService() {
        var intervals = ['years', 'months', 'days'],
            starts,
            ends,
            out = [];

        if ($groupStartDateElement.val()) {
            starts = $groupEndDateElement.val() ? moment($groupEndDateElement.val()) : moment(currentDate);
            ends = moment($groupStartDateElement.val());
            out = [];

            for (var i = 0; i < intervals.length; i++) {
                var diff = starts.diff(ends, intervals[i]);
                ends.add(diff, intervals[i]);
                out.push(diff + ' ' + intervals[i].replace('s', '(s)'));
            }

            $('#ServiceGroup').val(out.join('/'));
        }

        if ($companyStartDateElement.val()) {
            starts = $companyEndDateElement.val() ? moment($companyEndDateElement.val()) : moment(currentDate);
            ends = moment($companyStartDateElement.val());
            out = [];

            for (var i = 0; i < intervals.length; i++) {
                var diff = starts.diff(ends, intervals[i]);
                ends.add(diff, intervals[i]);
                out.push(diff + ' ' + intervals[i].replace('s', '(s)'));
            }

            $('#ServiceCompany').val(out.join('/'));
        }
    }

    function registerEventChangeDateCommencement() {
        $groupStartDateElement.on('dp.change', function (e) {
            calculateLengthOfService();
        });

        $groupEndDateElement.on('dp.change', function (e) {
            calculateLengthOfService();
        });

        $companyStartDateElement.on('dp.change', function (e) {
            calculateLengthOfService();
        });

        $companyEndDateElement.on('dp.change', function (e) {
            calculateLengthOfService();
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

    function initDatepicker($form) {
        $form.find('input.datetimepicker').each(function () {
            var $this = $(this),
                defaultDate = $this.val(),
                mindate = $this.data('mindate'),
                maxdate = $this.data('maxdate');

            $this.datetimepicker({
                format: 'MMM DD, YYYY',
                minDate: mindate,
                maxDate: maxdate,
                useCurrent: false
            });

            if (defaultDate) {
                defaultDate = moment(defaultDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
                $this.data("DateTimePicker").date(defaultDate);
            } else {
                $this.val('');
            }
        });

        $form.find('.icon-calendar:not(.disabled)').on('click', function () {
            $(this).siblings('.datetimepicker').data('DateTimePicker').show();
        });
    }

    function reparseform($form) {
        $form.removeData('validator').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse($form);

        var $validator = $form.validate();
        $validator.settings.errorPlacement = function (error, element) {
            if (error.text().trim().length > 0) {
                error.addClass('message');
                $form.find('.list-message-error').removeClass('hidden').append(error);
            }
        };
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

    function initEmploymentForm() {
        $createEmploymentForm = $('#createEmploymentForm');
        currentDate = $createEmploymentForm.find('#CurrentDate').val();
        $groupStartDateElement = $createEmploymentForm.find('#GroupStartDate');
        $groupEndDateElement = $createEmploymentForm.find('#GroupEndDate');
        $companyStartDateElement = $createEmploymentForm.find('#CompanyStartDate');
        $companyEndDateElement = $createEmploymentForm.find('#CompanyEndDate');

        initDatepicker($createEmploymentForm);
        calculateLengthOfService();
        $bviCompanyElement = $createEmploymentForm.find('#CompanyId');
        $divisionElement = $createEmploymentForm.find('#DivisionId');
        $departmentElement = $createEmploymentForm.find('#DepartmentId');
        $sectionElement = $createEmploymentForm.find('#SectionId');
        $costCenterElement = $createEmploymentForm.find('#CostCenterId');

        reparseform($createEmploymentForm);
        initSelect2($createEmploymentForm);
        registerEventChangeDateCommencement();
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
    }

    function initModule() {
        initEmploymentForm();
    }

    return {
        InitModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                $createEmploymentForm.find('.list-message-error').addClass('hidden');
            } else {
                if (response.errors) {
                    $createEmploymentForm.validate().showErrors(response.errors);
                } else {
                    $createEmploymentForm.find('.list-message-error').removeClass('hidden')
                        .append('<span class="message">' + response.message + '</span>');
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.InitModule();
});