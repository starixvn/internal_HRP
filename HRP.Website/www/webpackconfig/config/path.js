const path = require('path');
const { report } = require('process');

const projectRoot = path.resolve(__dirname, '../../../');

const cssPaths = {
    build: 'wwwroot/css',

    main: ['./www/sources/css/pages/main.scss'],
    error: ['./www/sources/css/pages/errors/error.scss'],
    // auth
    auth_before_login: ['./www/sources/css/pages/auth/before-login.scss'],
    auth_login: ['./www/sources/css/pages/auth/login.scss'],
    auth_otp: ['./www/sources/css/pages/auth/otp.scss'],
    // user
    user_index: ['./www/sources/css/pages/user/index.scss'],
    user_create: ['./www/sources/css/pages/user/create.scss'],
    user_assign_permission: ['./www/sources/css/pages/user/assign-permission.scss'],
    // data configuration
    data_configuration_index: ['./www/sources/css/pages/data-configuration/index.scss'],
    // currency setting
    currency_setting_index: ['./www/sources/css/pages/currency-setting/index.scss'],
    currency_setting_index: ['./www/sources/css/pages/currency-setting/index.scss'],
    // system security
    system_security_certificate: ['./www/sources/css/pages/system-security/certificate.scss'],
    system_security_changepassword: ['./www/sources/css/pages/system-security/changepassword.scss'],
    // mail setting
    mail_setting_index: ['./www/sources/css/pages/mail-setting/index.scss'],
    mail_setting_edit: ['./www/sources/css/pages/mail-setting/edit.scss'],
    // employee
    employee_index: ['./www/sources/css/pages/employee/index.scss'],
    employee_create: ['./www/sources/css/pages/employee/create.scss'],
    employee_payroll: ['./www/sources/css/pages/employee/payroll.scss'],
    employee_bank_details: ['./www/sources/css/pages/employee/bank-details.scss'],
    // payment process
    payment_process_index: ['./www/sources/css/pages/payment-process/index.scss'],
    payment_process_upload_attendance: ['./www/sources/css/pages/payment-process/upload-files.scss'],
    payment_process_monthly_payroll: ['./www/sources/css/pages/payment-process/monthly-payroll.scss'],
    payment_process_edit_basic_salary: ['./www/sources/css/pages/payment-process/edit-basic-salary.scss'],
    payment_process_edit_allowances: ['./www/sources/css/pages/payment-process/edit-allowances.scss'],
    // payslip
    payslip_index: ['./www/sources/css/pages/payslip/index.scss'],
    payslip_details: ['./www/sources/css/pages/payslip/details.scss'],
    payslip_pdf: ['./www/sources/css/pages/payslip/pdf.scss'],
    // report
    report_dynamic: ['./www/sources/css/pages/report/dynamic-report.scss'],
    report_monthly_payroll: ['./www/sources/css/pages/report/monthly-payroll-report.scss'],
    report_direct_banking: ['./www/sources/css/pages/report/direct-banking-report.scss'],
    report_ebanking: ['./www/sources/css/pages/report/ebanking-report.scss']
};

const scriptPaths = {
    build: 'wwwroot/js'
};

module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};