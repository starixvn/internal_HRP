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
        BtnDeleteDSTNetSalaryFile: '.js-deleteDSTNetSalary',
        IsHasAttendanceFileSelected: '#isHasAttendanceFileSelected',
        IsHasDSTNetSalaryFileSelected: '#isHasDSTNetSalaryFileSelected',
        FileNotUploadedWarning: '#fileNotUploadedWarning',
        NextAnyway: '.js-nextAnyway',
        BrowseFileWarning: '#browseFileWarning',
        IsShowBrowseFileWarning: '#isShowBrowseFileWarning',
        ContinueToAction: '#continueToAction', // 1:browse Attendance, 2:browse DSTNetSalary
        ConfirmBrowseFile: '.js-confirmBrowseFile'
    };

    function showErrorsValidationFile($form, inputName, message) {
        var errorArray = {};
        errorArray[inputName] = message;
        $form.validate().showErrors(errorArray);
    }

    function validateFileExcel($form, fileType, file) {
        var errorMessage;
        if (file === undefined) {
            errorMessage = '';
        } else {
            if (file.size / (1024 * 1024) > 5) {
                errorMessage = fileType + 'The upload file size can not exceed 5Mb. Please use a smaller file!';
            } else {
                var pass = /\.(xls|xlsx)$/i.test(file.name);
                if (!pass) {
                    errorMessage = fileType + 'The file type is not supported. Please try again.';
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
            showErrorsValidationFile($(domElement.UploadFilesForm), "AttendanceFile", '');
            showErrorsValidationFile($(domElement.UploadFilesForm), "DSTNetSalaryFile", '');
            $(domElement.IsHasAttendanceFileSelected).val(0);
            $(domElement.IsHasDSTNetSalaryFileSelected).val(0);
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

        hideLoadingAnimation();
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
            showLoadingAnimation();
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
            var errorMessage = validateFileExcel($form, "Attendance file: ", this.files[0]);
            showErrorsValidationFile($form, "AttendanceFile", errorMessage);
            if (errorMessage !== '') {
                $(this).val("");
            }

            var currentFile = this.files[0];
            if (currentFile) {
                $(domElement.AttendanceFileName).text(currentFile.name);
                $(domElement.AttendanceIconSuccess).removeClass('hidden').addClass('hidden');
                $(domElement.AttendanceIconClose).removeClass('hidden');

                $(domElement.IsHasAttendanceFileSelected).val(1);
            } else {
                clearAttendanceFilename();
                $(domElement.IsHasAttendanceFileSelected).val(0);
            }

            showHideButtonUploadFiles();
        });

        $(domElement.DSTNetSalaryFile).on('change', function () {
            var errorMessage = validateFileExcel($form, "DST Net Salary file: ", this.files[0]);
            showErrorsValidationFile($form, "DSTNetSalaryFile", errorMessage);
            if (errorMessage !== '') {
                $(this).val("");
            }

            var currentFile = this.files[0];
            if (currentFile) {
                $(domElement.DSTNetSalaryFileName).text(currentFile.name);
                $(domElement.DSTNetSalaryIconSuccess).removeClass('hidden').addClass('hidden');
                $(domElement.DSTNetSalaryIconClose).removeClass('hidden');
                $(domElement.IsHasDSTNetSalaryFileSelected).val(1);
            } else {
                clearDSTNetSalaryFilename();
                $(domElement.IsHasDSTNetSalaryFileSelected).val(0);
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
            if ($(domElement.IsShowBrowseFileWarning).val() === '1') {
                $(domElement.ContinueToAction).val(1);
                $(domElement.BrowseFileWarning).modal('show');
            } else {
                $(domElement.AttendanceFile).trigger('click');
            }
        });

        $(domElement.BrowseDSTNetSalary).on('click', function () {
            if ($(domElement.IsShowBrowseFileWarning).val() === '1') {
                $(domElement.ContinueToAction).val(2);
                $(domElement.BrowseFileWarning).modal('show');
            } else {
                $(domElement.DSTNetSalaryFile).trigger('click');
            }
        });
    }

    function registerEventConfirmBrowseFile() {
        $(domElement.ConfirmBrowseFile).on('click', function () {
            var action = $(domElement.ContinueToAction).val();
            switch (action) {
                case '1': //Browse Attendance file
                    $(domElement.AttendanceFile).trigger('click');
                    break;
                case '2': //Browse DST Net Salary file
                    $(domElement.DSTNetSalaryFile).trigger('click');
                    break;
            }
            $(domElement.IsShowBrowseFileWarning).val(0);
            $(domElement.BrowseFileWarning).modal('hide');
        });
    }

    function registerEventNextToMonthlyPayroll() {
        $(domElement.BtnNextToMonthlyPayroll).on('click', function () {
            var isAttendanceSelected = $(domElement.IsHasAttendanceFileSelected).val();
            var isDSTNetSalarySelected = $(domElement.IsHasDSTNetSalaryFileSelected).val();
            if (isAttendanceSelected === '0' && isDSTNetSalarySelected === '0') {
                checkPaymentProcessStatus();
            } else {
                $(domElement.FileNotUploadedWarning).modal('show');
            }
        });
    }

    function checkPaymentProcessStatus() {
        showLoadingAnimation();
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
                    if (response.isAttendanceFileSubmitted === false || response.isDSTNetSalaryFileSubmitted === false) {
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
                    redirectToPageEmployeeMonthlyPayroll();
                }

               // hideLoadingAnimation();
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function registerEventNextAnyway() {
        $(domElement.NextAnyway).on('click', function () {
            checkPaymentProcessStatus();
            $(domElement.FileNotUploadedWarning).modal('hide');
        });
    }

    function registerEventConfirmGeneratePayroll() {
        $(domElement.ConfirmGeneratePayroll).on('click', function () {
            processActionNextToMonthlyPayroll();
            $(domElement.GeneratePayrollWarning).modal('hide');
        });
    }

    function processActionNextToMonthlyPayroll() {
        showLoadingAnimation();
        var $companyId = $(domElement.CompanyId),
            $selectedTime = $(domElement.SelectedTime);

        $.ajax({
            type: 'POST',
            url: commonFunctions.rootURL + 'PaymentProcess/ProcessActionNextToMonthlyPayroll',
            dataType: 'json',
            data: {
                companyId: $companyId.val(),
                month: $selectedTime.val(),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', '#uploadFilesForm').val()
            },
            success: function (response) {
                if (response.isSuccessful) {
                    redirectToPageEmployeeMonthlyPayroll();
                } else {
                    $(domElement.UploadFilesForm).validate().showErrors(response.errors);
                    hideLoadingAnimation();
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
        showLoadingAnimation();
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

                hideLoadingAnimation();
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function redirectToPageEmployeeMonthlyPayroll() {
        var $companyId = $(domElement.CompanyId),
            $selectedTime = $(domElement.SelectedTime);
        window.location = commonFunctions.rootURL + 'PaymentProcess/EmployeeMonthlyPayroll?companyId=' + $companyId.val() + '&month=' + encodeURIComponent($selectedTime.val());
    }

    function showLoadingAnimation() {
        $('.block-ui').removeClass('hidden');
    }

    function hideLoadingAnimation() {
        $('.block-ui').removeClass('hidden').addClass('hidden');
    }

    function initDatatable() {
        $(domElement.AttendanceErrorsTable).DataTable({
            info: false,
            // dom: 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            ordering: false,
            filter: false,
            scrollX: false,
            scrollY: '145px',
            scrollCollapse: true,
            responsive: true,
            paging: false
        });

        $(domElement.DSTNetSalaryErrorsTable).DataTable({
            info: false,
            // dom: 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            ordering: false,
            filter: false,
            scrollY: '145px',
            scrollCollapse: true,
            responsive: true,
            paging: false,
            scrollX: false
        });
    }

    function init() {
        var $form = $(domElement.UploadFilesForm);
        $('select.js-select2').select2();

        registerEventChangeExcelFiles($form);
        registerEventBrowseExcelFiles();
        registerEventClearFiles();
        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
        registerEventSubmitForm($form);
        registerEventDeleteUploadedFiles();
        registerEventNextToMonthlyPayroll();
        registerEventConfirmGeneratePayroll();
        registerEventNextAnyway();
        registerEventConfirmBrowseFile();
    }

    return {
        init: init,
        initDatatable: initDatatable
    };
}(jQuery));

$(document).ready(function () {
    uploadAttendanceAndSalary.init();
});

if (document.readyState === 'loading') {
    uploadAttendanceAndSalary.initDatatable();
}