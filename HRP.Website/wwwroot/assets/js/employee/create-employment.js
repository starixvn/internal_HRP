'use strict';

var createEmployment = (function ($) {
    var $personalDetailsStep, $employmentStep,
        $personalDetailHeaderTab, $employmentHeaderTab, $employmentContentTab, $createEmploymentForm,
        $companyCodeElement, $employeeCodeElement, $pdfPasswordElement, $btnBack, $btnCancel;

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

    function displaySuccessMessage() {
        var $element = $createEmploymentForm.find('.msg-success'),
            $countDownElement = $element.find('.count-down');
        var delay = 5;
        $element.removeClass('hidden');

        var intervalCountDown = setInterval(function () {
            if (delay === 0) {
                clearInterval(intervalCountDown);
                $btnCancel.click();
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
                commonFunctions.hideValidationSummary($createEmploymentForm);

                var formData = new FormData(document.forms['personalDetailsForm']);
                var poData = jQuery(document.forms['createEmploymentForm']).serializeArray();
                for (var i = 0; i < poData.length; i++) {
                    formData.append(poData[i].name, poData[i].value);
                }

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'EmployeeManagement/Create/Employment',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (response) {
                        if (response.isSuccessful) {
                            displaySuccessMessage();
                        } else {
                            if (response.errors) {
                                $createEmploymentForm.validate().showErrors(response.errors);
                                commonFunctions.showValidationSummary($createEmploymentForm);
                            } else {
                                commonFunctions.showErrorMessage($createEmploymentForm.find('.msg-error'), response.message);
                            }
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($createEmploymentForm);
            }

            $('.content-container').scrollTop(0);
        });
    }

    function loadEmploymentForm() {
        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'EmployeeManagement/Create/Employment',
            dataType: 'html',
            success: function (data) {
                $employmentContentTab.html(data);

                $createEmploymentForm = $('#createEmploymentForm');
                $companyCodeElement = $createEmploymentForm.find('#CompanyCode');
                $employeeCodeElement = $createEmploymentForm.find('#EmployeeCode');
                $pdfPasswordElement = $createEmploymentForm.find('#PDFPassword');
                $btnBack = $createEmploymentForm.find('.js-btnBack');
                $btnCancel = $createEmploymentForm.find('.js-btnCancel');

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