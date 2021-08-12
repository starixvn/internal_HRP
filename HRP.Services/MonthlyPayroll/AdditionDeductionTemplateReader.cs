namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Services;
    using HRP.Models.Currency;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using NPOI.SS.UserModel;

    public interface IAdditionDeductionTemplateReader
    {
        Result<AdditionDeductionTemplate> ReadFile(AdditionDeductionTemplate template);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    [SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated", Justification = "Pending")]
    public class AdditionDeductionTemplateReader : IAdditionDeductionTemplateReader
    {
        private readonly ICryptoService cryptoService;
        private readonly DataFormatter formatter;
        private readonly IList<AdditionDeductionUploading> records;
        private readonly IList<ValidationResult> validationResults;
        private const int StartRowIndex = 4;

        private readonly Dictionary<int, string> FirstColumnHeaders = new Dictionary<int, string>
        {
            { 1, "Monthly Basic Salary" }, { 18, "Monthly Allowances" }
        };

        private readonly Dictionary<int, string> SecondColumnHeaders = new Dictionary<int, string>
        {
            { 1, "Addition" }, { 10, "Deduction" }, { 18, "Addition" }, { 48, "Deduction" }
        };
        private readonly Dictionary<int, string> ThirdColumnHeaders = new Dictionary<int, string>
        {
            { 1, "Adjustments" }, { 4, "Others 1" }, { 7, "Others 2" }, { 10, "UPL/Absenteeism" }, { 12, "Tardiness" }, { 14, "Salary Advance" },
            { 16, "Others" }, { 18, "Subsistence" }, { 21, "Home Passage" }, { 24, "Housing" }, { 27, "Mobile Phone" }, { 30, "HPI" },
            { 33, "Duty Meal" }, { 35, "Secondment" }, { 38, "Special Allowances" }, { 41, "OT" }, { 44, "Others" }, { 47, "Comment" },
            { 48, "Subsistence" }, { 51, "Home Passage" }, { 54, "Housing" }, { 57, "Mobile Phone" }, { 60, "HPI" },
            { 63, "Duty Meal" }, { 65, "Secondment" }, { 68, "Others" }, { 71, "Comment" }
        };
        private readonly Dictionary<int, string> FourthColumnHeaders = new Dictionary<int, string>
        {
            { 0, "ID No" }, { 1, "Comment" }, { 2, "Amount" }, { 3, "No.of Days" }, { 4, "Comment" }, { 5, "Amount" }, { 6, "No.of Days" },
            { 7, "Comment" }, { 8, "Amount" }, { 9, "No.of Days" }, { 10, "Comment" }, { 11, "Days" },
            { 12, "Comment" }, { 13, "Minutes" }, { 14, "Comment" }, { 15, "Amount" }, { 16, "Comment" }, { 17, "Amount" },
            { 18, "Amount" }, { 19, "Currency Code" }, { 20, "No.of Days" }, { 21, "Amount" }, { 22, "Currency Code" }, { 23, "No.of Days" },
            { 24, "Amount" }, { 25, "Currency Code" }, { 26, "No.of Days" }, { 27, "Amount" }, { 28, "Currency Code" }, { 29, "No.of Days" },
            { 30, "Amount" }, { 31, "Currency Code" }, { 32, "No.of Days" }, { 33, "Currency Code" }, { 34, "No.of Days" },
            { 35, "Amount" }, { 36, "Currency Code" }, { 37, "No.of Days" }, { 38, "Amount" }, { 39, "Currency Code" }, { 40, "No.of Days" },
            { 41, "Amount" }, { 42, "Currency Code" }, { 43, "No.of Days" }, { 44, "Amount" }, { 45, "Currency Code" }, { 46, "No.of Days" },
            { 48, "Amount" }, { 49, "Currency Code" }, { 50, "No.of Days" }, { 51, "Amount" }, { 52, "Currency Code" }, { 53, "No.of Days" },
            { 54, "Amount" }, { 55, "Currency Code" }, { 56, "No.of Days" }, { 57, "Amount" }, { 58, "Currency Code" }, { 59, "No.of Days" },
            { 60, "Amount" }, { 61, "Currency Code" }, { 62, "No.of Days" }, { 63, "Currency Code" }, { 64, "No.of Days" },
            { 65, "Amount" }, { 66, "Currency Code" }, { 67, "No.of Days" }, { 68, "Amount" }, { 69, "Currency Code" }, { 70, "No.of Days" }
        };

        public AdditionDeductionTemplateReader(ICryptoService cryptoService, DataFormatter formatter = null)
        {
            this.cryptoService = cryptoService;
            this.formatter = formatter ?? new DataFormatter();
            records = new List<AdditionDeductionUploading>();
            validationResults = new List<ValidationResult>();
        }

        public Result<AdditionDeductionTemplate> ReadFile(AdditionDeductionTemplate template)
        {
            try
            {
                if (!IsValidSheet(template.Sheet))
                {
                    return new Failure<AdditionDeductionTemplate>(Message.IncorrectTemplate);
                }

                for (var i = StartRowIndex; i <= template.Sheet.LastRowNum; i++)
                {
                    var row = template.Sheet.GetRow(i);
                    if (IsValidRow(row))
                    {
                        var rowNumber = row.RowNum + 1;
                        var record = ReadRow(row, rowNumber);
                        ValidateRow(template, record);

                        record.Salary_Addition_Adjustments_Amount = cryptoService.Encrypt(record.Salary_Addition_Adjustments_Amount);
                        record.Salary_Addition_Others1_Amount = cryptoService.Encrypt(record.Salary_Addition_Others1_Amount);
                        record.Salary_Addition_Others2_Amount = cryptoService.Encrypt(record.Salary_Addition_Others2_Amount);
                        record.Salary_Deduction_Advance_Amount = cryptoService.Encrypt(record.Salary_Deduction_Advance_Amount);
                        record.Salary_Deduction_Others_Amount = cryptoService.Encrypt(record.Salary_Deduction_Others_Amount);

                        record.Allowance_Addition_Subsistence_Amount = cryptoService.Encrypt(record.Allowance_Addition_Subsistence_Amount);
                        record.Allowance_Addition_HomePassage_Amount = cryptoService.Encrypt(record.Allowance_Addition_HomePassage_Amount);
                        record.Allowance_Addition_Housing_Amount = cryptoService.Encrypt(record.Allowance_Addition_Housing_Amount);
                        record.Allowance_Addition_MobilePhone_Amount = cryptoService.Encrypt(record.Allowance_Addition_MobilePhone_Amount);
                        record.Allowance_Addition_HPI_Amount = cryptoService.Encrypt(record.Allowance_Addition_HPI_Amount);
                        record.Allowance_Addition_Secondment_Amount = cryptoService.Encrypt(record.Allowance_Addition_Secondment_Amount);
                        record.Allowance_Addition_SpecialAllowance_Amount = cryptoService.Encrypt(record.Allowance_Addition_SpecialAllowance_Amount);
                        record.Allowance_Addition_OT_Amount = cryptoService.Encrypt(record.Allowance_Addition_OT_Amount);
                        record.Allowance_Addition_Others_Amount = cryptoService.Encrypt(record.Allowance_Addition_Others_Amount);

                        record.Allowance_Deduction_Subsistence_Amount = cryptoService.Encrypt(record.Allowance_Deduction_Subsistence_Amount);
                        record.Allowance_Deduction_HomePassage_Amount = cryptoService.Encrypt(record.Allowance_Deduction_HomePassage_Amount);
                        record.Allowance_Deduction_Housing_Amount = cryptoService.Encrypt(record.Allowance_Deduction_Housing_Amount);
                        record.Allowance_Deduction_MobilePhone_Amount = cryptoService.Encrypt(record.Allowance_Deduction_MobilePhone_Amount);
                        record.Allowance_Deduction_HPI_Amount = cryptoService.Encrypt(record.Allowance_Deduction_HPI_Amount);
                        record.Allowance_Deduction_Secondment_Amount = cryptoService.Encrypt(record.Allowance_Deduction_Secondment_Amount);
                        record.Allowance_Deduction_Others_Amount = cryptoService.Encrypt(record.Allowance_Deduction_Others_Amount);

                        records.Add(record);
                    }
                }

                template.Records = records;
                template.ValidationResults = validationResults;

                return new Success<AdditionDeductionTemplate>(template);
            }
            catch (Exception)
            {
                return new Failure<AdditionDeductionTemplate>(Message.ErrorOccurredWhileReadingTemplate);
            }
        }

        private AdditionDeductionUploading ReadRow(IRow row, int rowNumber)
        {
            return new AdditionDeductionUploading
            {
                RowNumber = rowNumber,
                EmployeeCode = GetStringValueAtCell(0, row),
                Salary_Addition_Adjustments_Comment = GetStringValueAtCell(1, row),
                Salary_Addition_Adjustments_Amount = GetStringValueAtCell(2, row),
                Salary_Addition_Adjustments_NoOfDays = GetStringValueAtCell(3, row),

                Salary_Addition_Others1_Comment = GetStringValueAtCell(4, row),
                Salary_Addition_Others1_Amount = GetStringValueAtCell(5, row),
                Salary_Addition_Others1_NoOfDays = GetStringValueAtCell(6, row),
                Salary_Addition_Others2_Comment = GetStringValueAtCell(7, row),
                Salary_Addition_Others2_Amount = GetStringValueAtCell(8, row),
                Salary_Addition_Others2_NoOfDays = GetStringValueAtCell(9, row),

                Salary_Deduction_Absenteeism_Comment = GetStringValueAtCell(10, row),
                Salary_Deduction_Absenteeism_Days = GetStringValueAtCell(11, row),
                Salary_Deduction_Tardiness_Comment = GetStringValueAtCell(12, row),
                Salary_Deduction_Tardiness_Minutes = GetStringValueAtCell(13, row),
                Salary_Deduction_Advance_Comment = GetStringValueAtCell(14, row),
                Salary_Deduction_Advance_Amount = GetStringValueAtCell(15, row),
                Salary_Deduction_Others_Comment = GetStringValueAtCell(16, row),
                Salary_Deduction_Others_Amount = GetStringValueAtCell(17, row),

                Allowance_Addition_Subsistence_Amount = GetStringValueAtCell(18, row),
                Allowance_Addition_Subsistence_CurrencyCode = GetStringValueAtCell(19, row),
                Allowance_Addition_Subsistence_NoOfDays = GetStringValueAtCell(20, row),
                Allowance_Addition_HomePassage_Amount = GetStringValueAtCell(21, row),
                Allowance_Addition_HomePassage_CurrencyCode = GetStringValueAtCell(22, row),
                Allowance_Addition_HomePassage_NoOfDays = GetStringValueAtCell(23, row),
                Allowance_Addition_Housing_Amount = GetStringValueAtCell(24, row),
                Allowance_Addition_Housing_CurrencyCode = GetStringValueAtCell(25, row),
                Allowance_Addition_Housing_NoOfDays = GetStringValueAtCell(26, row),
                Allowance_Addition_MobilePhone_Amount = GetStringValueAtCell(27, row),
                Allowance_Addition_MobilePhone_CurrencyCode = GetStringValueAtCell(28, row),
                Allowance_Addition_MobilePhone_NoOfDays = GetStringValueAtCell(29, row),
                Allowance_Addition_HPI_Amount = GetStringValueAtCell(30, row),
                Allowance_Addition_HPI_CurrencyCode = GetStringValueAtCell(31, row),
                Allowance_Addition_HPI_NoOfDays = GetStringValueAtCell(32, row),
                Allowance_Addition_DutyMeal_CurrencyCode = GetStringValueAtCell(33, row),
                Allowance_Addition_DutyMeal_NoOfDays = GetStringValueAtCell(34, row),
                Allowance_Addition_Secondment_Amount = GetStringValueAtCell(35, row),
                Allowance_Addition_Secondment_CurrencyCode = GetStringValueAtCell(36, row),
                Allowance_Addition_Secondment_NoOfDays = GetStringValueAtCell(37, row),
                Allowance_Addition_SpecialAllowance_Amount = GetStringValueAtCell(38, row),
                Allowance_Addition_SpecialAllowance_CurrencyCode = GetStringValueAtCell(39, row),
                Allowance_Addition_SpecialAllowance_NoOfDays = GetStringValueAtCell(40, row),
                Allowance_Addition_OT_Amount = GetStringValueAtCell(41, row),
                Allowance_Addition_OT_CurrencyCode = GetStringValueAtCell(42, row),
                Allowance_Addition_OT_NoOfDays = GetStringValueAtCell(43, row),
                Allowance_Addition_Others_Amount = GetStringValueAtCell(44, row),
                Allowance_Addition_Others_CurrencyCode = GetStringValueAtCell(45, row),
                Allowance_Addition_Others_NoOfDays = GetStringValueAtCell(46, row),
                Allowance_Addition_Comment = GetStringValueAtCell(47, row),

                Allowance_Deduction_Subsistence_Amount = GetStringValueAtCell(48, row),
                Allowance_Deduction_Subsistence_CurrencyCode = GetStringValueAtCell(49, row),
                Allowance_Deduction_Subsistence_NoOfDays = GetStringValueAtCell(50, row),
                Allowance_Deduction_HomePassage_Amount = GetStringValueAtCell(51, row),
                Allowance_Deduction_HomePassage_CurrencyCode = GetStringValueAtCell(52, row),
                Allowance_Deduction_HomePassage_NoOfDays = GetStringValueAtCell(53, row),
                Allowance_Deduction_Housing_Amount = GetStringValueAtCell(54, row),
                Allowance_Deduction_Housing_CurrencyCode = GetStringValueAtCell(55, row),
                Allowance_Deduction_Housing_NoOfDays = GetStringValueAtCell(56, row),
                Allowance_Deduction_MobilePhone_Amount = GetStringValueAtCell(57, row),
                Allowance_Deduction_MobilePhone_CurrencyCode = GetStringValueAtCell(58, row),
                Allowance_Deduction_MobilePhone_NoOfDays = GetStringValueAtCell(59, row),
                Allowance_Deduction_HPI_Amount = GetStringValueAtCell(60, row),
                Allowance_Deduction_HPI_CurrencyCode = GetStringValueAtCell(61, row),
                Allowance_Deduction_HPI_NoOfDays = GetStringValueAtCell(62, row),
                Allowance_Deduction_DutyMeal_CurrencyCode = GetStringValueAtCell(63, row),
                Allowance_Deduction_DutyMeal_NoOfDays = GetStringValueAtCell(64, row),
                Allowance_Deduction_Secondment_Amount = GetStringValueAtCell(65, row),
                Allowance_Deduction_Secondment_CurrencyCode = GetStringValueAtCell(66, row),
                Allowance_Deduction_Secondment_NoOfDays = GetStringValueAtCell(67, row),
                Allowance_Deduction_Others_Amount = GetStringValueAtCell(68, row),
                Allowance_Deduction_Others_CurrencyCode = GetStringValueAtCell(69, row),
                Allowance_Deduction_Others_NoOfDays = GetStringValueAtCell(70, row),
                Allowance_Deduction_Comment = GetStringValueAtCell(71, row)
            };
        }

        private void ValidateRow(AdditionDeductionTemplate template, AdditionDeductionUploading record)
        {
            const short CommentMaxLength = 200;
            AddToValidationResults(CheckEmployeeCode(template, record.RowNumber, record.EmployeeCode));
            AddToValidationResults(CheckComment(record.RowNumber, 1, record.Salary_Addition_Adjustments_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 2, record.Salary_Addition_Adjustments_Amount));
            AddToValidationResults(CheckAmount(record.RowNumber, 3, record.Salary_Addition_Adjustments_NoOfDays));
            AddToValidationResults(CheckComment(record.RowNumber, 4, record.Salary_Addition_Others1_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 5, record.Salary_Addition_Others1_Amount));
            AddToValidationResults(CheckAmount(record.RowNumber, 6, record.Salary_Addition_Others1_NoOfDays));
            AddToValidationResults(CheckComment(record.RowNumber, 7, record.Salary_Addition_Others2_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 8, record.Salary_Addition_Others2_Amount));
            AddToValidationResults(CheckAmount(record.RowNumber, 9, record.Salary_Addition_Others2_NoOfDays));

            AddToValidationResults(CheckComment(record.RowNumber, 10, record.Salary_Deduction_Absenteeism_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 11, record.Salary_Deduction_Absenteeism_Days));
            AddToValidationResults(CheckComment(record.RowNumber, 12, record.Salary_Deduction_Tardiness_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 13, record.Salary_Deduction_Tardiness_Minutes));
            AddToValidationResults(CheckComment(record.RowNumber, 14, record.Salary_Deduction_Advance_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 15, record.Salary_Deduction_Advance_Amount));
            AddToValidationResults(CheckComment(record.RowNumber, 16, record.Salary_Deduction_Others_Comment, CommentMaxLength));
            AddToValidationResults(CheckAmount(record.RowNumber, 17, record.Salary_Deduction_Others_Amount));

            AddToValidationResults(CheckAmount(record.RowNumber, 18, record.Allowance_Addition_Subsistence_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 19, record.Allowance_Addition_Subsistence_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 20, record.Allowance_Addition_Subsistence_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 21, record.Allowance_Addition_HomePassage_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 22, record.Allowance_Addition_HomePassage_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 23, record.Allowance_Addition_HomePassage_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 24, record.Allowance_Addition_Housing_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 25, record.Allowance_Addition_Housing_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 26, record.Allowance_Addition_Housing_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 27, record.Allowance_Addition_MobilePhone_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 28, record.Allowance_Addition_MobilePhone_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 29, record.Allowance_Addition_MobilePhone_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 30, record.Allowance_Addition_HPI_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 31, record.Allowance_Addition_HPI_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 32, record.Allowance_Addition_HPI_NoOfDays));
            AddToValidationResults(CheckCurrency(record.RowNumber, 33, record.Allowance_Addition_DutyMeal_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 34, record.Allowance_Addition_DutyMeal_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 35, record.Allowance_Addition_Secondment_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 36, record.Allowance_Addition_Secondment_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 37, record.Allowance_Addition_Secondment_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 38, record.Allowance_Addition_SpecialAllowance_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 39, record.Allowance_Addition_SpecialAllowance_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 40, record.Allowance_Addition_SpecialAllowance_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 41, record.Allowance_Addition_OT_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 42, record.Allowance_Addition_OT_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 43, record.Allowance_Addition_OT_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 44, record.Allowance_Addition_Others_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 45, record.Allowance_Addition_Others_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 46, record.Allowance_Addition_Others_NoOfDays));

            AddToValidationResults(CheckAmount(record.RowNumber, 48, record.Allowance_Deduction_Subsistence_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 49, record.Allowance_Deduction_Subsistence_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 50, record.Allowance_Deduction_Subsistence_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 51, record.Allowance_Deduction_HomePassage_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 52, record.Allowance_Deduction_HomePassage_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 53, record.Allowance_Deduction_HomePassage_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 54, record.Allowance_Deduction_Housing_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 55, record.Allowance_Deduction_Housing_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 56, record.Allowance_Deduction_Housing_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 57, record.Allowance_Deduction_MobilePhone_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 58, record.Allowance_Deduction_MobilePhone_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 59, record.Allowance_Deduction_MobilePhone_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 60, record.Allowance_Deduction_HPI_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 61, record.Allowance_Deduction_HPI_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 62, record.Allowance_Deduction_HPI_NoOfDays));
            AddToValidationResults(CheckCurrency(record.RowNumber, 63, record.Allowance_Deduction_DutyMeal_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 64, record.Allowance_Deduction_DutyMeal_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 65, record.Allowance_Deduction_Secondment_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 66, record.Allowance_Deduction_Secondment_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 67, record.Allowance_Deduction_Secondment_NoOfDays));
            AddToValidationResults(CheckAmount(record.RowNumber, 68, record.Allowance_Deduction_Others_Amount));
            AddToValidationResults(CheckCurrency(record.RowNumber, 69, record.Allowance_Deduction_Others_CurrencyCode, template.Currencies));
            AddToValidationResults(CheckAmount(record.RowNumber, 70, record.Allowance_Deduction_Others_NoOfDays));
        }

        private ValidationResult CheckEmployeeCode(AdditionDeductionTemplate template, int rowNumber, string value)
        {
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (string.IsNullOrWhiteSpace(value))
            {
                result.ResultType = (short)ValidationResultType.Error;
                result.Message = string.Format(Message.ValueMissing, FourthColumnHeaders[0]);
            }
            else if (!template.EmployeeCodes.Any(s => string.Equals(value, s, StringComparison.OrdinalIgnoreCase)))
            {
                result.ResultType = (short)ValidationResultType.Error;
                result.Message = string.Format(Message.IDNoNotExistIn, template.CompanyCode);
            }
            else if (records.Any(s => string.Equals(value, s.EmployeeCode, StringComparison.OrdinalIgnoreCase)))
            {
                result.ResultType = (short)ValidationResultType.Error;
                result.Message = Message.IDNoIsDuplicated;
            }
            else
            {
                result.ResultType = (int)ValidationResultType.Success;
            }

            return result;
        }

        private ValidationResult CheckCurrency(int rowNumber, int colIndex, string value, IEnumerable<Currency> currencies)
        {
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (!string.IsNullOrWhiteSpace(value) && !currencies.Any(x => string.Equals(value, x.CurrencyCode, StringComparison.OrdinalIgnoreCase)))
            {
                result.ResultType = (short)ValidationResultType.Error;
                result.Message = string.Format("Column {0} - {1}", colIndex, Message.CurrencyNotExists);
            }
            else
            {
                result.ResultType = (short)ValidationResultType.Success;
            }

            return result;
        }

        private ValidationResult CheckAmount(int rowNumber, int colIndex, string value)
        {
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (!string.IsNullOrWhiteSpace(value) && !decimal.TryParse(value, out decimal _))
            {
                result.ResultType = (short)ValidationResultType.Error;
                result.Message = string.Format("Column {0} - {1} must be a number.", colIndex, FourthColumnHeaders[colIndex]);
            }
            else
            {
                result.ResultType = (short)ValidationResultType.Success;
            }

            return result;
        }

        private ValidationResult CheckComment(int rowNumber, int colIndex, string value, int maxLength)
        {
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
            {
                result.ResultType = (short)ValidationResultType.Error;
                result.Message = string.Format("Column {0} - {1} must not over {2} characters long.", colIndex, FourthColumnHeaders[colIndex], maxLength);
            }
            else
            {
                result.ResultType = (short)ValidationResultType.Success;
            }

            return result;
        }

        private void AddToValidationResults(ValidationResult result)
        {
            if (!result.IsValid)
            {
                validationResults.Add(result);
            }
        }

        private static Dictionary<int, string> GetHeaders(ISheet sheet, int headerRow)
        {
            return sheet
                .GetRow(headerRow)
                .Cells
                .Where(cell => !string.IsNullOrWhiteSpace(cell.StringCellValue))
                .ToDictionary(cell => cell.ColumnIndex, cell => cell.StringCellValue.Trim().ToLower(CultureInfo.InvariantCulture));
        }

        private bool IsValidSheet(ISheet sheet)
        {
            var firstHeaders = GetHeaders(sheet, 0);
            if (FirstColumnHeaders.Count(item => firstHeaders[item.Key].Equals(item.Value.ToLower(CultureInfo.InvariantCulture)))
                != FirstColumnHeaders.Count)
            {
                return false;
            }

            var secondHeaders = GetHeaders(sheet, 1);
            if (SecondColumnHeaders.Count(item => secondHeaders[item.Key].Equals(item.Value.ToLower(CultureInfo.InvariantCulture)))
                != SecondColumnHeaders.Count)
            {
                return false;
            }

            var thirdHeaders = GetHeaders(sheet, 2);
            if (ThirdColumnHeaders.Count(item => thirdHeaders[item.Key].Equals(item.Value.ToLower(CultureInfo.InvariantCulture)))
                != ThirdColumnHeaders.Count)
            {
                return false;
            }

            var fourthHeaders = GetHeaders(sheet, 3);
            if (FourthColumnHeaders.Count(item => fourthHeaders[item.Key].Equals(item.Value.ToLower(CultureInfo.InvariantCulture)))
                != FourthColumnHeaders.Count)
            {
                return false;
            }

            return true;
        }

        private static bool IsValidRow(IRow row)
        {
            return !IsEmptyRow(row);
        }

        private static bool IsEmptyRow(IRow row)
        {
            return
                row == null
                || row.Cells.TrueForAll(c => c.CellType == CellType.Blank || (c.CellType == CellType.String && string.IsNullOrWhiteSpace(c.StringCellValue)))
                || row.Cells.Count == 0;
        }

        private string GetStringValueAtCell(int columnIndex, IRow row)
        {
            var value = (string)GetPropValue(columnIndex, row, typeof(string));
            return value?.Trim();
        }

        private static Type GetBasicType(Type t)
        {
            var tmp = t;

            if (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                tmp = t.GetGenericArguments()[0];
            }

            return tmp;
        }

        private object GetPropValue(int columnIndex, IRow row, Type rowType)
        {
            var t = GetBasicType(rowType);
            var value = CellReader(row, columnIndex, t);

            if (rowType.IsGenericType)
            {
                if (t == typeof(int))
                {
                    return (int)value;
                }

                if (t == typeof(double))
                {
                    return (double)value;
                }

                return (string)value;
            }

            return value;
        }

        private object CellReader(IRow row, int idx, Type t)
        {
            var cell = row.GetCell(idx);

            if (t == typeof(int))
            {
                return ReadInt(cell);
            }

            if (t == typeof(double))
            {
                return ReadDouble(cell);
            }

            return ReadString(cell);
        }

        private static object ReadInt(ICell cell)
        {
            if (cell == null)
            {
                return default(int);
            }

            return cell.CellType == CellType.String ? int.Parse(cell.StringCellValue, CultureInfo.InvariantCulture) : (int)cell.NumericCellValue;
        }

        private static object ReadDouble(ICell cell)
        {
            if (cell == null)
            {
                return default(double);
            }

            return cell.CellType == CellType.String ? double.Parse(cell.StringCellValue, CultureInfo.InvariantCulture) : cell.NumericCellValue;
        }

        private object ReadString(ICell cell)
        {
            var value = formatter.FormatCellValue(cell);

            return value ?? string.Empty;
        }
    }
}