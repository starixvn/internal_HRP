'use strict';

var createEmployment = (function ($) {
    var $personalDetailsStep, $employmentStep,
        $personalDetailHeaderTab, $employmentHeaderTab, $employmentContentTab, $createEmploymentForm,
        $companyCodeElement, $employeeCodeElement, $pdfPasswordElement, $btnBack;

    function initEventChangeCompanyCode() {
        $companyCodeElement.on('change', function () {
            $pdfPasswordElement.val($companyCodeElement.val().trim() + $employeeCodeElement.val().trim()).valid();
        });
    }

    function initEventChangeEmployeeCode() {
        $employeeCodeElement.on('change', function () {
            $pdfPasswordElement.val($companyCodeElement.val().trim() + $employeeCodeElement.val().trim()).valid();
        });
    }

    function initEventBackToPreviousAction() {
        $btnBack.on('click', function () {
            $personalDetailsStep.removeClass('finished').addClass('active');
            $employmentStep.removeClass('active');
            $personalDetailHeaderTab.trigger('click');
        });
    }

    function displaySuccessMessage(objectId) {
        var $personalDetailsForm = $('#personalDetailsForm');
        var $element = $createEmploymentForm.find('.msg-success'),
            $countDownElement = $element.find('.count-down');
        var delay = 3;
        $element.removeClass('hidden');

        var intervalCountDown = setInterval(function () {
            if (delay === 0) {
                clearInterval(intervalCountDown);
                window.location.href = commonFunctions.rootURL + 'Employee/Payroll/' + objectId
                    + '?departmentIds=' + $personalDetailsForm.find('#Filter_DepartmentIds').val()
                    + '&employmentStatusIds=' + $personalDetailsForm.find('#Filter_EmploymentStatusIds').val()
                    + '&employeeId=' + $personalDetailsForm.find('#Filter_EmployeeId').val()
                    + '&displayName=' + $personalDetailsForm.find('#Filter_EmployeeName').val();
            } else {
                delay--;
                $countDownElement.html(delay);
            }
        }, 1000);
    }

    function registerEventSubmitForm() {
        $createEmploymentForm.submit(function (e) {
            e.preventDefault();

            employmentCommonFuncs.initSelect2($createEmploymentForm);

            if ($(this).valid()) {
                var formData = new FormData(document.forms['personalDetailsForm']);
                var poData = jQuery(document.forms['createEmploymentForm']).serializeArray();
                for (var i = 0; i < poData.length; i++) {
                    formData.append(poData[i].name, poData[i].value);
                }

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'Employee/Create/Employment',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (response) {
                        if (response.isSuccessful) {
                            displaySuccessMessage(response.objectId);
                        } else {
                            if (response.errors) {
                                $createEmploymentForm.validate().showErrors(response.errors);
                            } else {
                                commonFunctions.showErrorMessage($createEmploymentForm.find('.msg-error'), response.message);
                            }
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            }

            $('.content-container').scrollTop(0);
        });
    }

    function loadEmploymentForm() {
        var $personalDetailsForm = $('#personalDetailsForm');

        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'Employee/Create/Employment',
            dataType: 'html',
            data: {
                departmentIds: $personalDetailsForm.find('#Filter_DepartmentIds').val(),
                employmentStatusIds: $personalDetailsForm.find('#Filter_EmploymentStatusIds').val(),
                employeeId: $personalDetailsForm.find('#Filter_EmployeeId').val(),
                employeeName: $personalDetailsForm.find('#Filter_EmployeeName').val()
            },
            success: function (data) {
                $employmentContentTab.html(data);

                $createEmploymentForm = $('#createEmploymentForm');
                $companyCodeElement = $createEmploymentForm.find('#CompanyCode');
                $employeeCodeElement = $createEmploymentForm.find('#EmployeeCode');
                $pdfPasswordElement = $createEmploymentForm.find('#PDFPassword');
                $btnBack = $createEmploymentForm.find('.js-btnBack');

                employmentCommonFuncs.initForm($createEmploymentForm);

                initEventChangeCompanyCode();
                initEventChangeEmployeeCode();
                initEventBackToPreviousAction();
                registerEventSubmitForm();
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function initEventClickEmploymentTab() {
        $employmentHeaderTab.on('click', function () {
            var isFirstLoad = $(this).data('isfirstload');

            if (isFirstLoad) {
                $(this).data('isfirstload', false);
                loadEmploymentForm();
            }
        });
    }

    function initModule() {
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');
        $personalDetailHeaderTab = $('#PersonalDetailHeaderTab');
        $employmentHeaderTab = $('#EmploymentHeaderTab');
        $employmentContentTab = $('#EmploymentContentTab');

        initEventClickEmploymentTab();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.initModule();
});