const alias = require('../config/alias');
const pathConfig = require('../config/path');
const pluginGenerator = require('../config/plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

function generateConfig(environment) {
    const jsPath = `${pathConfig.scriptPaths.build}/${environment}`;
    const cssPath = `${pathConfig.cssPaths.build}/${environment}`;
    const commonConfigs = {
        resolve: {
            alias: alias,
            extensions: ['.js', '.jsx']
        },
        entry: {
            /*js*/
            [`${jsPath}/main`]: pathConfig.scriptPaths.main,
            [`${jsPath}/home`]: pathConfig.scriptPaths.home,
            [`${jsPath}/pages/auth/login`]: pathConfig.scriptPaths.auth_login,
            [`${jsPath}/pages/auth/reset-password`]: pathConfig.scriptPaths.auth_reset_password,
            [`${jsPath}/pages/account/index`]: pathConfig.scriptPaths.account_index,
            [`${jsPath}/pages/account/setup-otp`]: pathConfig.scriptPaths.account_setup_otp,
            [`${jsPath}/pages/account/verify-otp`]: pathConfig.scriptPaths.account_verify_otp,
            // user
            [`${jsPath}/pages/user/index`]: pathConfig.scriptPaths.user_index,
            [`${jsPath}/pages/user/create`]: pathConfig.scriptPaths.user_create,
            [`${jsPath}/pages/user/edit`]: pathConfig.scriptPaths.user_edit,
            [`${jsPath}/pages/user/assign-permission`]: pathConfig.scriptPaths.user_assign_permission,
            [`${jsPath}/pages/data-configuration/index`]: pathConfig.scriptPaths.data_configuration_index,
            [`${jsPath}/pages/mail-setting/edit`]: pathConfig.scriptPaths.mail_setting_edit,
            [`${jsPath}/pages/currency-setting/index`]: pathConfig.scriptPaths.currency_setting_index,
            [`${jsPath}/pages/system-security/change-password`]: pathConfig.scriptPaths.system_security_change_password,
            [`${jsPath}/pages/system-security/generate-cert-file`]: pathConfig.scriptPaths.system_security_generate_cert_file,
            [`${jsPath}/pages/system-security/input-password`]: pathConfig.scriptPaths.system_security_input_password,
            // employee
            [`${jsPath}/pages/employee/index`]: pathConfig.scriptPaths.employee_index,
            [`${jsPath}/pages/employee/create`]: pathConfig.scriptPaths.employee_create,
            [`${jsPath}/pages/employee/edit`]: pathConfig.scriptPaths.employee_edit,
            [`${jsPath}/pages/employee/payroll`]: pathConfig.scriptPaths.employee_payroll,
            // bank account
            [`${jsPath}/pages/bank-account/index`]: pathConfig.scriptPaths.bank_account_index,
            [`${jsPath}/pages/bank-account/create`]: pathConfig.scriptPaths.bank_account_create,
            // payment process
            [`${jsPath}/pages/payment-process/index`]: pathConfig.scriptPaths.payment_process_index,
            [`${jsPath}/pages/payment-process/upload-attendance`]: pathConfig.scriptPaths.payment_process_upload_attendance,
            [`${jsPath}/pages/payment-process/monthly-payroll`]: pathConfig.scriptPaths.payment_process_monthly_payroll,
            [`${jsPath}/pages/payment-process/edit-basic-salary`]: pathConfig.scriptPaths.payment_process_edit_basic_salary,
            [`${jsPath}/pages/payment-process/edit-allowances`]: pathConfig.scriptPaths.payment_process_edit_allowances,
            // payslip
            [`${jsPath}/pages/payslip/index`]: pathConfig.scriptPaths.payslip_index,
            [`${jsPath}/pages/payslip/details`]: pathConfig.scriptPaths.payslip_details,
            // report
            [`${jsPath}/pages/report/dynamic`]: pathConfig.scriptPaths.report_dynamic,
            [`${jsPath}/pages/report/monthly-payroll`]: pathConfig.scriptPaths.report_monthly_payroll,
            [`${jsPath}/pages/report/payment-transaction`]: pathConfig.scriptPaths.report_payment_transaction,

            /*css*/
            [`${cssPath}/main`]: pathConfig.cssPaths.main,
            [`${cssPath}/error`]: pathConfig.cssPaths.error,
            // auth
            [`${cssPath}/pages/auth/before-login`]: pathConfig.cssPaths.auth_before_login,
            [`${cssPath}/pages/auth/login`]: pathConfig.cssPaths.auth_login,
            [`${cssPath}/pages/auth/otp`]: pathConfig.cssPaths.auth_otp,
            // user
            [`${cssPath}/pages/user/index`]: pathConfig.cssPaths.user_index,
            [`${cssPath}/pages/user/create`]: pathConfig.cssPaths.user_create,
            [`${cssPath}/pages/user/assign-permission`]: pathConfig.cssPaths.user_assign_permission,
            [`${cssPath}/pages/data-configuration/index`]: pathConfig.cssPaths.data_configuration_index,
            [`${cssPath}/pages/currency-setting/index`]: pathConfig.cssPaths.currency_setting_index,
            [`${cssPath}/pages/system-security/certificate`]: pathConfig.cssPaths.system_security_certificate,
            [`${cssPath}/pages/system-security/changepassword`]: pathConfig.cssPaths.system_security_changepassword,
            // mail setting
            [`${cssPath}/pages/mail-setting/index`]: pathConfig.cssPaths.mail_setting_index,
            [`${cssPath}/pages/mail-setting/edit`]: pathConfig.cssPaths.mail_setting_edit,
            // employee
            [`${cssPath}/pages/employee/index`]: pathConfig.cssPaths.employee_index,
            [`${cssPath}/pages/employee/create`]: pathConfig.cssPaths.employee_create,
            [`${cssPath}/pages/employee/payroll`]: pathConfig.cssPaths.employee_payroll,
            [`${cssPath}/pages/employee/bank-details`]: pathConfig.cssPaths.employee_bank_details,
            // payment process
            [`${cssPath}/pages/payment-process/index`]: pathConfig.cssPaths.payment_process_index,
            [`${cssPath}/pages/payment-process/upload-attendance`]: pathConfig.cssPaths.payment_process_upload_attendance,
            [`${cssPath}/pages/payment-process/monthly-payroll`]: pathConfig.cssPaths.payment_process_monthly_payroll,
            [`${cssPath}/pages/payment-process/edit-basic-salary`]: pathConfig.cssPaths.payment_process_edit_basic_salary,
            [`${cssPath}/pages/payment-process/edit-allowances`]: pathConfig.cssPaths.payment_process_edit_allowances,
            // payslip
            [`${cssPath}/pages/payslip/index`]: pathConfig.cssPaths.payslip_index,
            [`${cssPath}/pages/payslip/details`]: pathConfig.cssPaths.payslip_details,
            [`${cssPath}/pages/payslip/pdf`]: pathConfig.cssPaths.payslip_pdf,
            // report
            [`${cssPath}/pages/report/dynamic`]: pathConfig.cssPaths.report_dynamic,
            [`${cssPath}/pages/report/monthly-payroll`]: pathConfig.cssPaths.report_monthly_payroll,
            [`${cssPath}/pages/report/direct-banking`]: pathConfig.cssPaths.report_direct_banking,
            [`${cssPath}/pages/report/ebanking`]: pathConfig.cssPaths.report_ebanking
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js",
            chunkFilename: `${jsPath}/lib/[name].chunk.js`,
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    loader: ["babel-loader"],
                    exclude: /node_modules/
                }
            ]
        },
        plugins: pluginGenerator(environment),
        stats: {
            assets: false,
            colors: true,
            version: false,
            hash: false,
            timings: true,
            chunks: false,
            chunkModules: false,
            children: true
        },
        optimization: {
            minimizer: [
                new UglifyJsPlugin({
                    sourceMap: true,
                    uglifyOptions: {
                        output: {
                            comments: false
                        }
                    }
                }),
                new OptimizeCssAssetsPlugin()
            ],
            splitChunks: {
                chunks: 'all',
                minSize: 30000,
                maxSize: 0,
                minChunks: 2,
                maxAsyncRequests: 5,
                maxInitialRequests: 3,
                name: false,
                cacheGroups: {
                    // disable default cache group
                    default: false,
                    vendors: false,
                    reactVendor: {
                        name: 'react-vendor',
                        test: /[\\/]node_modules[\\/](react|react-dom)[\\/]/,
                        minChunks: 2,
                        priority: -10
                    },
                    reactBootstrapVendor: {
                        name: 'react-bootstrap-vendor',
                        test: /[\\/]node_modules[\\/](react-bootstrap)[\\/]/,
                        minChunks: 2,
                        priority: -10
                    }
                }
            }
        }
    };

    return commonConfigs;
}

module.exports = generateConfig;