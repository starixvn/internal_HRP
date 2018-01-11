'use strict';

var createEmployment = (function ($) {
    var $employmentHeaderTab, $employmentTab, $createEmploymentForm,
        $bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement;

    function loadEmploymentForm() {
        $employmentHeaderTab.on('click', function () {
            var $this = $(this);
            var isFirstLoad = $this.data('isfirstload');

            if (isFirstLoad) {
                $this.data('isfirstload', false);

                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'EmployeeManagement/CreateEmployment',
                    dataType: 'html',
                    data: {
                        employeeId: 1
                    },
                    success: function (data) {
                        $employmentTab.html(data);

                        $createEmploymentForm = $('#createEmploymentForm');
                        $bviCompanyElement = $('#CompanyId');
                        $divisionElement = $('#DivisionId');
                        $departmentElement = $('#DepartmentId');
                        $sectionElement = $('#SectionId');
                        $costCenterElement = $('#CostCenterId');

                        employmentCommonFunctions.reparseform($createEmploymentForm);
                        employmentCommonFunctions.initDatepicker($createEmploymentForm);
                        employmentCommonFunctions.initSelect2($createEmploymentForm);
                        employmentCommonFunctions.calculateLengthOfService();

                        employmentCommonFunctions.registerEventChangeBVICompany($bviCompanyElement, $divisionElement, $departmentElement, $sectionElement, $costCenterElement);
                        employmentCommonFunctions.registerEventChangeDivision($divisionElement, $departmentElement, $sectionElement, $costCenterElement);
                        employmentCommonFunctions.registerEventChangeDepartment($departmentElement, $sectionElement, $costCenterElement);
                        employmentCommonFunctions.registerEventChangeSection($sectionElement, $costCenterElement);
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoPFileAdmin'), $('#FullNamePFileAdmin'), $('#NickNamePFileAdmin'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoBVISalary'), $('#Paymaster_FullNameBVISalary'), $('#Paymaster_NickNameBVISalary'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoBVIAllowance'), $('#Paymaster_FullNameBVIAllowance'), $('#Paymaster_NickNameBVIAllowance'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoLocalSalary'), $('#Paymaster_FullNameLocalSalary'), $('#Paymaster_NickNameLocalSalary'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoAdministrator'), $('#EPA_FullNameAdministrator'), $('#EPA_NickNameAdministrator'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoEvaluator1'), $('#EPA_FullNameEvaluator1'), $('#EPA_NickNameEvaluator1'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoEvaluator2'), $('#EPA_FullNameEvaluator2'), $('#EPA_NickNameEvaluator2'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoReviewer'), $('#EPA_FullNameReviewer'), $('#EPA_NickNameReviewer'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoManagement'), $('#EPA_FullNameManagement'), $('#EPA_NickNameManagement'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoIncrement'), $('#EPA_FullNameIncrement'), $('#EPA_NickNameIncrement'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoBVIBonus'), $('#EPA_FullNameBVIBonus'), $('#EPA_NickNameBVIBonus'));
                        employmentCommonFunctions.initSearchAutocomplete($('#NoneOrIDNoLocalBonus'), $('#EPA_FullNameLocalBonus'), $('#EPA_NickNameLocalBonus'));
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
        onCreateSuccess: function (response) {
            if (response.isSuccessful) {
                $('.validation-summary-errors').addClass('hidden');
                $('.validation-summary-errors ul').html('');
            } else {
                if (response.errors) {
                    $createEmploymentForm.validate().showErrors(response.errors);
                } else {
                    $('.validation-summary-errors').removeClass('hidden');
                    $('.validation-summary-errors ul').html('<li>' + response.message + '</li>');
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.InitModule();
});