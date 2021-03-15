import moment from 'moment';

window.employmentCommonFuncs = (function ($) {
    let $bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement,
        $groupStartDateElement, $groupEndDateElement, $companyStartDateElement, $companyEndDateElement,
        $companyCodeElement, $jobCategoryElement, $gradeLevelElement, $jobGradeElement,
        defaultOption = '<option value=""></option>',
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
                url: commonFunctions.rootURL + 'Employee/GetDivisionByCompanyId',
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
                url: commonFunctions.rootURL + 'Employee/GetDepartmentByDivisionId',
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
                url: commonFunctions.rootURL + 'Employee/GetCostCenterByDepartmentId',
                dataType: 'json',
                data: {
                    departmentId: departmentId
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
                    // reset
                    $sectionElement.html(defaultOption);
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventChangeCostCenter() {
        $costCenterElement.on('change', function () {
            var costCenterId = $(this).val(),
                htmlOptions = '';

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'Employee/GetSectionByCostCenterId',
                dataType: 'json',
                data: {
                    costCenterId: costCenterId
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
                url: commonFunctions.rootURL + 'Employee/GetGradeLevelByJobCategoryId',
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
                url: commonFunctions.rootURL + 'Employee/GetJobGradeByGradeLevelId',
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
        $form.find('.js-select2-hidesearchbox').select2({
            placeholder: '--Please select--',
            minimumResultsForSearch: Infinity
        });

        $form.find('.js-search-suggestion').select2({
            placeholder: '--Please select--'
        });

        $form.find('.js-select2-hidesearchbox-hideplaceholder').select2({
            placeholder: '',
            minimumResultsForSearch: Infinity
        });

        $form.find('.js-select2-allowclear-hidesearchbox-hideplaceholder').select2({
            placeholder: '',
            allowClear: true,
            minimumResultsForSearch: Infinity
        });
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

    function initSearchAutocomplete($selectorName, $selectorFullName, $selectorNickname) {
        $selectorName.autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');

                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: commonFunctions.rootURL + 'Employee/GetEmployeeSuggestion',
                    dataType: 'json',
                    data: {
                        idNo: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.nickname + ' (' + item.fullName + ')',
                                    fullname: item.fullName,
                                    nickname: item.nickname,
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
                $selectorName.val(ui.item.value);
                $selectorFullName.val(ui.item.fullname);
                $selectorNickname.val(ui.item.nickname);
                $selectorFullName.valid();
                $selectorNickname.valid();

                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
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

            if ($element.hasClass('js-select2')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(errorClass).addClass('control');
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('js-select2')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(validClass);
            }
        };

        $form.find('.js-select2').on('change', function () {
            var $this = $(this);
            var $select2 = $this.next().find('.select2-selection');

            if ($this.val() && $select2.hasClass('input-validation-error')) {
                $select2.removeClass('input-validation-error').removeClass('control');
                $this.valid();
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
        registerEventChangeCostCenter();
        registerEventChangeJobCategory();
        registerEventChangeGradeLevel();
        initSearchAutocomplete($('#NoneOrIDNoPFileAdmin'), $('#FullNamePFileAdmin'), $('#NicknamePFileAdmin'));
        initSearchAutocomplete($('#NoneOrIDNoBVISalary'), $('#Paymaster_FullNameBVISalary'), $('#Paymaster_NicknameBVISalary'));
        initSearchAutocomplete($('#NoneOrIDNoBVIAllowance'), $('#Paymaster_FullNameBVIAllowance'), $('#Paymaster_NicknameBVIAllowance'));
        initSearchAutocomplete($('#NoneOrIDNoLocalSalary'), $('#Paymaster_FullNameLocalSalary'), $('#Paymaster_NicknameLocalSalary'));
        initSearchAutocomplete($('#NoneOrIDNoAdministrator'), $('#EPA_FullNameAdministrator'), $('#EPA_NicknameAdministrator'));
        initSearchAutocomplete($('#NoneOrIDNoEvaluator1'), $('#EPA_FullNameEvaluator1'), $('#EPA_NicknameEvaluator1'));
        initSearchAutocomplete($('#NoneOrIDNoEvaluator2'), $('#EPA_FullNameEvaluator2'), $('#EPA_NicknameEvaluator2'));
        initSearchAutocomplete($('#NoneOrIDNoReviewer'), $('#EPA_FullNameReviewer'), $('#EPA_NicknameReviewer'));
        initSearchAutocomplete($('#NoneOrIDNoManagement'), $('#EPA_FullNameManagement'), $('#EPA_NicknameManagement'));
        initSearchAutocomplete($('#NoneOrIDNoIncrement'), $('#EPA_FullNameIncrement'), $('#EPA_NicknameIncrement'));
        initSearchAutocomplete($('#NoneOrIDNoBVIBonus'), $('#EPA_FullNameBVIBonus'), $('#EPA_NicknameBVIBonus'));
        initSearchAutocomplete($('#NoneOrIDNoLocalBonus'), $('#EPA_FullNameLocalBonus'), $('#EPA_NicknameLocalBonus'));

        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
    }

    return {
        initForm: initForm,
        initSelect2: initSelect2
    };
}(jQuery));