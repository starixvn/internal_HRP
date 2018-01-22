'use strict';

var employmentCommonFuncs = (function ($) {
    var $bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement,
        $groupStartDateElement, $groupEndDateElement, $companyStartDateElement, $companyEndDateElement,
        $companyCodeElement, $jobCategoryElement, $gradeLevelElement, $jobGradeElement,
        defaultOption = '<option value="" style="display: none;">--Please select--</option>',
        currentDate;

    function registerEventChangeBVICompany() {
        $bviCompanyElement.on('change', function () {
            var companyId = $(this).val(),
                companyCode = $(this).find('option:selected').text(),
                htmlOptions = '';

            $companyCodeElement.val(companyCode);
            $companyCodeElement.trigger('change');

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetDivisionByCompanyId',
                dataType: 'json',
                data: {
                    companyId: companyId
                },
                success: function (data) {
                    htmlOptions = defaultOption;

                    if (data && data.length > 0) {
                        var totalItems = data.length;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].divisionId + '">' + data[i].code + '</option>';
                        }
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
                    htmlOptions = defaultOption;

                    if (data && data.length > 0) {
                        var totalItems = data.length;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].departmentId + '">' + data[i].code + '</option>';
                        }
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
                    htmlOptions = defaultOption;

                    if (data && data.length > 0) {
                        var totalItems = data.length;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].sectionId + '">' + data[i].code + '</option>';
                        }
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
                    htmlOptions = defaultOption;

                    if (data && data.length > 0) {
                        var totalItems = data.length;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].costCenterId + '">' + data[i].code + '</option>';
                        }
                    }

                    $costCenterElement.html(htmlOptions);
                    $costCenterElement.select2('destroy').select2();
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeJobCategory() {
        $jobCategoryElement.on('change', function () {
            var jobCategoryId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetGradeLevelByJobCategoryId',
                dataType: 'json',
                data: {
                    jobCategoryId: jobCategoryId
                },
                success: function (data) {
                    htmlOptions = defaultOption;

                    if (data && data.length > 0) {
                        var totalItems = data.length;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].gradeLevelId + '">' + data[i].name + '</option>';
                        }
                    }

                    $gradeLevelElement.html(htmlOptions);
                    // reset
                    $jobGradeElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeGradeLevel() {
        $gradeLevelElement.on('change', function () {
            var gradeLevelId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'EmployeeManagement/GetJobGradeByGradeLevelId',
                dataType: 'json',
                data: {
                    gradeLevelId: gradeLevelId
                },
                success: function (data) {
                    htmlOptions = defaultOption;

                    if (data && data.length > 0) {
                        var totalItems = data.length;

                        for (var i = 0; i < totalItems; i++) {
                            htmlOptions += '<option value="' + data[i].jobGradeId + '">' + data[i].name + '</option>';
                        }
                    }

                    $jobGradeElement.html(htmlOptions);
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
            starts, ends,
            i, diff, out = [];

        if ($groupStartDateElement.val()) {
            starts = $groupEndDateElement.val() ? moment($groupEndDateElement.val()) : moment(currentDate);
            ends = moment($groupStartDateElement.val());
            out = [];

            for (i = 0; i < intervals.length; i++) {
                diff = starts.diff(ends, intervals[i]);
                ends.add(diff, intervals[i]);
                out.push(diff + ' ' + intervals[i].replace('s', '(s)'));
            }

            $('#ServiceGroup').val(out.join('/'));
        }

        if ($companyStartDateElement.val()) {
            starts = $companyEndDateElement.val() ? moment($companyEndDateElement.val()) : moment(currentDate);
            ends = moment($companyStartDateElement.val());
            out = [];

            for (i = 0; i < intervals.length; i++) {
                diff = starts.diff(ends, intervals[i]);
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

    function reparseform($form) {
        $form.removeData('validator').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse($form);

        var $validator = $form.validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);

            if ($element.hasClass('search-suggestion')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(errorClass).addClass('control');
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('search-suggestion')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(validClass);
            }
        };

        $(document).on('change', 'select.search-suggestion', function () {
            if (!$.isEmptyObject($validator.submitted)) {
                $form.find('.select2-selection').removeClass('input-validation-error').removeClass('control');
                $validator.form();
            }
        });
    }

    function registerEventFormChange($form) {
        $form.on('change keyup click', function () {
            var errorTotal = $form.find('.list-message-error .field-validation-error').length;
            if (errorTotal === 0) {
                commonFunctions.hideValidationSummary($form);
            } else {
                commonFunctions.showValidationSummary($form);
            }
        });
    }

    function initForm($form) {
        currentDate = $form.find('#CurrentDate').val();
        $groupStartDateElement = $form.find('#GroupStartDate');
        $groupEndDateElement = $form.find('#GroupEndDate');
        $companyStartDateElement = $form.find('#CompanyStartDate');
        $companyEndDateElement = $form.find('#CompanyEndDate');

        commonFunctions.initDatetimepicker($form);
        calculateLengthOfService();
        $bviCompanyElement = $form.find('#CompanyId');
        $divisionElement = $form.find('#DivisionId');
        $departmentElement = $form.find('#DepartmentId');
        $sectionElement = $form.find('#SectionId');
        $costCenterElement = $form.find('#CostCenterId');
        $companyCodeElement = $form.find('#CompanyCode');
        $jobCategoryElement = $form.find('#JobCategoryId');
        $gradeLevelElement = $form.find('#GradeLevelId');
        $jobGradeElement = $form.find('#JobGradeId');

        reparseform($form);
        initSelect2($form);
        registerEventChangeDateCommencement();
        registerEventChangeBVICompany();
        registerEventChangeDivision();
        registerEventChangeDepartment();
        registerEventChangeSection();
        registerEventChangeJobCategory();
        registerEventChangeGradeLevel();
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

        registerEventFormChange($form);
    }

    return {
        initForm: initForm,
        initSelect2: initSelect2
    };
}(jQuery));