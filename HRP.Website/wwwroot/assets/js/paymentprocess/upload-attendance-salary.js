var uploadAttendanceAndSalary = (function ($) {
    var domElement = {
        BtnUploadFiles: '#uploadFiles',
        UploadFilesForm: '#uploadFilesForm',
        CompanyId: '#CompanyId',
        SelectedTime: '#SelectedTime',
        AttendanceFile: '#AttendanceFile',
        DSTNetSalaryFile: '#DSTNetSalaryFile',
        ValidationFilesResult: '#validationFilesResult',
        BrowseAttendance: '.js-browseAttendance',
        BrowseDSTNetSalary: '.js-browseDSTNetSalary',
        AttendanceFileName: '.js-attendanceFileName',
        DSTNetSalaryFileName: '.js-dstNetSalaryFileName',
        AttendanceIconSuccess: '.js-attendanceIconSuccess',
        DSTNetSalaryIconSuccess: '.js-dstNetSalaryIconSuccess',
        AttendanceIconClose: '.js-attendanceIconClose',
        DSTNetSalaryIconClose: '.js-dstNetSalaryIconClose',
        AttendanceErrorsTable: '.js-attendanceErrorsTable',
        DSTNetSalaryErrorsTable: '.js-dstNetSalaryErrorsTable',
        AttendanceTotalError: '.js-attendanceTotalError',
        DSTNetSalaryTotalError: '.js-dstNetSalaryTotalError',
        BtnNextToMonthlyPayroll: '#nextToMonthlyPayroll',
        GeneratePayrollWarning: '#generatePayrollWarning',
        ConfirmGeneratePayroll: '.js-confirmGeneratePayroll',
        BtnDeleteAttendanceFile: '.js-deleteAttendance',
        BtnDeleteDSTNetSalaryFile: '.js-deleteDSTNetSalary'
    };

    function showErrorsValidationFile($form, inputName, message) {
        var errorArray = {};
        errorArray[inputName] = message;
        $form.validate().showErrors(errorArray);
    }

    function validateFileExcel($form, inputName, file) {
        var errorMessage;
        if (file === undefined) {
            errorMessage = '';
        } else {
            if (file.size / (1024 * 1024) > 5) {
                errorMessage = 'The upload file size can not exceed 5Mb. Please use a smaller file!';
            } else {
                var pass = /\.(xls|xlsx)$/i.test(file.name);
                if (!pass) {
                    errorMessage = 'The file type is not supported. Please try again.';
                } else {
                    errorMessage = '';
                }
            }
        }

        return errorMessage;
    }

    function onUploadSuccess(response) {
        if (response.isSuccessful) {
            ////Note: can reduce these JS code if reload page :D
            renderValidationFilesResult();

            $(domElement.AttendanceFile).val("");
            $(domElement.DSTNetSalaryFile).val("");
            var attendanceFileName = $(domElement.AttendanceFileName);
            var dstNetSalaryFileName = $(domElement.DSTNetSalaryFileName);

            if (attendanceFileName.text() === '') {
                var oldAttendanceFileName = attendanceFileName.data('filename-uploaded');
                attendanceFileName.text(oldAttendanceFileName);
                if (oldAttendanceFileName !== '') {
                    showAttendanceIconSuccess();
                }
            } else {
                attendanceFileName.data('filename-uploaded', attendanceFileName.text());
                showAttendanceIconSuccess();
                $(domElement.BtnDeleteAttendanceFile).prop('disabled', false);
            }

            if (dstNetSalaryFileName.text() === '') {
                var oldDSTNetSalaryFileName = dstNetSalaryFileName.data('filename-uploaded');
                dstNetSalaryFileName.text(oldDSTNetSalaryFileName);
                if (oldDSTNetSalaryFileName !== '') {
                    showDSTNetSalaryIconSuccess();
                }
            } else {
                dstNetSalaryFileName.data('filename-uploaded', dstNetSalaryFileName.text());
                showDSTNetSalaryIconSuccess();
                $(domElement.BtnDeleteDSTNetSalaryFile).prop('disabled', false);
            }

            showHideButtonUploadFiles();
        } else {
            $(domElement.UploadFilesForm).validate().showErrors(response.errors);
        }
    }

    function renderValidationFilesResult() {
        var $companyId = $(domElement.CompanyId),
            $selectedTime = $(domElement.SelectedTime);

        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'PaymentProcess/ValidationFilesResult',
            dataType: 'html',
            data: {
                "companyId": $companyId.val(),
                "month": $selectedTime.val()
            },
            success: function (response) {
                $(domElement.ValidationFilesResult).html(response);
                initDatatable();
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function registerEventSubmitForm($form) {
        $form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'PaymentProcess/UploadAttendanceAndSalary',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: onUploadSuccess,
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEventChangeExcelFiles($form) {
        $(domElement.AttendanceFile).on('change', function () {
            var errorMessage = validateFileExcel($form, "AttendanceFile", this.files[0]);
            showErrorsValidationFile($form, "AttendanceFile", errorMessage);
            if (errorMessage !== '') {
                $(this).val("");
            }

            var currentFile = this.files[0];
            if (currentFile) {
                $(domElement.AttendanceFileName).text(currentFile.name);
                $(domElement.AttendanceIconSuccess).removeClass('hidden').addClass('hidden');
                $(domElement.AttendanceIconClose).removeClass('hidden');
            } else {
                clearAttendanceFilename();
            }

            showHideButtonUploadFiles();
        });

        $(domElement.DSTNetSalaryFile).on('change', function () {
            var errorMessage = validateFileExcel($form, "DSTNetSalaryFile", this.files[0]);
            showErrorsValidationFile($form, "DSTNetSalaryFile", errorMessage);
            if (errorMessage !== '') {
                $(this).val("");
            }

            var currentFile = this.files[0];
            if (currentFile) {
                $(domElement.DSTNetSalaryFileName).text(currentFile.name);
                $(domElement.DSTNetSalaryIconSuccess).removeClass('hidden').addClass('hidden');
                $(domElement.DSTNetSalaryIconClose).removeClass('hidden');
            } else {
                clearDSTNetSalaryFilename();
            }

            showHideButtonUploadFiles();
        });
    }

    function clearAttendanceFilename() {
        $(domElement.AttendanceFileName).text("");
        $(domElement.AttendanceIconSuccess).removeClass('hidden').addClass('hidden');
        $(domElement.AttendanceIconClose).removeClass('hidden').addClass('hidden');
    }

    function clearDSTNetSalaryFilename() {
        $(domElement.DSTNetSalaryFileName).text("");
        $(domElement.DSTNetSalaryIconSuccess).removeClass('hidden').addClass('hidden');
        $(domElement.DSTNetSalaryIconClose).removeClass('hidden').addClass('hidden');
    }

    function showAttendanceIconSuccess() {
        $(domElement.AttendanceIconSuccess).removeClass('hidden');
        $(domElement.AttendanceIconClose).removeClass('hidden').addClass('hidden');
    }

    function showDSTNetSalaryIconSuccess() {
        $(domElement.DSTNetSalaryIconSuccess).removeClass('hidden');
        $(domElement.DSTNetSalaryIconClose).removeClass('hidden').addClass('hidden');
    }

    function showHideButtonUploadFiles() {
        var attendanceFile = $(domElement.AttendanceFile).prop('files')[0];
        var dstNetSalaryFile = $(domElement.DSTNetSalaryFile).prop('files')[0];
        var btnUploadFiles = $(domElement.BtnUploadFiles);

        if (attendanceFile === undefined && dstNetSalaryFile === undefined) {
            btnUploadFiles.prop('disabled', true);
        } else {
            btnUploadFiles.prop('disabled', false);
        }
    }

    function registerEventClearFiles() {
        $(domElement.AttendanceIconClose).on('click', function () {
            $(domElement.AttendanceFile).val("");
            $(domElement.AttendanceFile).trigger('change');
        });

        $(domElement.DSTNetSalaryIconClose).on('click', function () {
            $(domElement.DSTNetSalaryFile).val("");
            $(domElement.DSTNetSalaryFile).trigger('change');
        });
    }

    function registerEventBrowseExcelFiles() {
        $(domElement.BrowseAttendance).on('click', function () {
            $(domElement.AttendanceFile).trigger('click');
        });

        $(domElement.BrowseDSTNetSalary).on('click', function () {
            $(domElement.DSTNetSalaryFile).trigger('click');
        });
    }

    function registerEventNextToMonthlyPayroll() {
        $(domElement.BtnNextToMonthlyPayroll).on('click', function () {
            var $companyId = $(domElement.CompanyId),
                $selectedTime = $(domElement.SelectedTime);

            $.ajax({
                type: 'GET',
                url: commonFunctions.rootURL + 'PaymentProcess/GetPaymentProcess',
                dataType: 'json',
                data: {
                    "companyId": $companyId.val(),
                    "month": $selectedTime.val()
                },
                success: function (response) {
                    if (response.isPayrollGenerated === false) {
                        if (response.isAttendanceFileSubmited === false || response.isDSTNetSalaryFileSubmited === false) {
                            var attendanceTotalError = $(domElement.AttendanceTotalError).val();
                            var dstNetSalaryTotalError = $(domElement.DSTNetSalaryTotalError).val();
                            if (attendanceTotalError > 0 || dstNetSalaryTotalError > 0) {
                                $(domElement.GeneratePayrollWarning).modal('show');
                            } else {
                                processActionNextToMonthlyPayroll();
                            }
                        } else {
                            processActionNextToMonthlyPayroll();
                        }
                    } else {
                        alert('go to monthly payroll');
                    }
                },
                error: commonFunctions.handleAjaxError
            });
        });
    }

    function registerEventConfirmGeneratePayroll() {
        $(domElement.ConfirmGeneratePayroll).on('click', function () {
            processActionNextToMonthlyPayroll();
            $(domElement.GeneratePayrollWarning).modal('hide');
        });
    }

    function processActionNextToMonthlyPayroll() {
        var $companyId = $(domElement.CompanyId),
            $selectedTime = $(domElement.SelectedTime);

        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'PaymentProcess/ProcessActionNextToMonthlyPayroll',
            dataType: 'json',
            data: {
                "companyId": $companyId.val(),
                "month": $selectedTime.val()
            },
            success: function (response) {
                if (response.isSuccessful) {
                    alert('generate successful!');
                } else {
                    $(domElement.UploadFilesForm).validate().showErrors(response.errors);
                    alert('generate fail !!!!!');
                }
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function registerEventDeleteUploadedFiles() {
        $(domElement.BtnDeleteAttendanceFile).on('click', function () {
            deleteUploadedFile('PaymentProcess/DeleteUploadedAttendance');
        });

        $(domElement.BtnDeleteDSTNetSalaryFile).on('click', function () {
            deleteUploadedFile('PaymentProcess/DeleteUploadedDSTNetSalary');
        });
    }

    function deleteUploadedFile(urlDelete) {
        var $companyId = $(domElement.CompanyId),
            $selectedTime = $(domElement.SelectedTime);

        $.ajax({
            type: 'POST',
            url: commonFunctions.rootURL + urlDelete,
            dataType: 'json',
            data: {
                companyId: $companyId.val(),
                month: $selectedTime.val(),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', '#uploadFilesForm').val()
            },
            success: function (response) {
                if (response.isSuccessful) {
                    window.location = commonFunctions.rootURL + 'PaymentProcess/UploadAttendanceAndSalary?companyId=' + $companyId.val() + '&month=' + encodeURIComponent($selectedTime.val());
                } else {
                    $(domElement.UploadFilesForm).validate().showErrors(response.errors);
                }
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function initDatatable() {
        $(domElement.AttendanceErrorsTable).DataTable({
            info: false,
            ordering: false,
            filter: false,
            scrollY: '145px',
            scrollCollapse: true,
            paging: false,
            scrollX: false
        });

        $(domElement.DSTNetSalaryErrorsTable).DataTable({
            info: false,
            ordering: false,
            filter: false,
            scrollY: '145px',
            scrollCollapse: true,
            paging: false,
            scrollX: false
        });
    }

    function init() {
        initDatatable();
        var $form = $(domElement.UploadFilesForm);
        registerEventChangeExcelFiles($form);
        registerEventBrowseExcelFiles();
        registerEventClearFiles();
        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
        registerEventSubmitForm($form);
        registerEventDeleteUploadedFiles();
        registerEventNextToMonthlyPayroll();
        registerEventConfirmGeneratePayroll();
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    uploadAttendanceAndSalary.init();
});