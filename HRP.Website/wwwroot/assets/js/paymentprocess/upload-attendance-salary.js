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
        DSTNetSalaryIconClose: '.js-dstNetSalaryIconClose'
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
            renderValidationFilesResult();

            ////TODO: Must consider: 1. JS load component upload 2. js render all page
            $(domElement.AttendanceFile).val("");
            $(domElement.DSTNetSalaryFile).val("");
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

    function registerEventChangeFileExcel($form) {
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
                showAttendanceFileUploaded();
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
                showDSTNetSalaryFileUploaded();
            }

            showHideButtonUploadFiles();
        });
    }

    function showAttendanceFileUploaded() {
        var fileName = $(domElement.AttendanceFileName).data('filename-uploaded');
        if (fileName) {
            $(domElement.AttendanceFileName).text(fileName);
            $(domElement.AttendanceIconSuccess).removeClass('hidden');
            $(domElement.AttendanceIconClose).removeClass('hidden').addClass('hidden');
        } else {
            $(domElement.AttendanceFileName).text("");
            $(domElement.AttendanceIconSuccess).removeClass('hidden').addClass('hidden');
            $(domElement.AttendanceIconClose).removeClass('hidden').addClass('hidden');
        }
    }

    function showDSTNetSalaryFileUploaded() {
        var fileName = $(domElement.DSTNetSalaryFileName).data('filename-uploaded');
        if (fileName) {
            $(domElement.DSTNetSalaryFileName).text(fileName);
            $(domElement.DSTNetSalaryIconSuccess).removeClass('hidden');
            $(domElement.DSTNetSalaryIconClose).removeClass('hidden').addClass('hidden');
        } else {
            $(domElement.DSTNetSalaryFileName).text("");
            $(domElement.DSTNetSalaryIconSuccess).removeClass('hidden').addClass('hidden');
            $(domElement.DSTNetSalaryIconClose).removeClass('hidden').addClass('hidden');
        }
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

    function registerEventBrowseFileExcel() {
        $(domElement.BrowseAttendance).on('click', function () {
            $(domElement.AttendanceFile).trigger('click');
        });

        $(domElement.BrowseDSTNetSalary).on('click', function () {
            $(domElement.DSTNetSalaryFile).trigger('click');
        });
    }

    function init() {
        var $form = $(domElement.UploadFilesForm);
        registerEventChangeFileExcel($form);
        registerEventBrowseFileExcel();
        registerEventClearFiles();
        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
        registerEventSubmitForm($form);
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    uploadAttendanceAndSalary.init();
});