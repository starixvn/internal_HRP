namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Extensions;
    using HRP.Core.Services;
    using HRP.Models.Currency;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using NPOI.SS.UserModel;
    using NSubstitute;
    using Xunit;

    public class TemplateReaderTests
    {
        private const int AttendanceStartRow = 6;
        private const int AttendanceHeaderRow = 5;
        private const int DSTNetSalaryStartRow = 5;
        private const int DSTNetSalaryHeaderRow = 4;
        private ICryptoService cryptoService;

        #region ReadAttendance

        [Fact]
        public void ReadAttendance_WhenThrowException_ReturnFailureResult()
        {
            // Arrange
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(new AttendanceTemplate());

            // Assert
            var result = Assert.IsType<Failure<AttendanceTemplate>>(actual);
            Assert.Equal(Message.ErrorOccuredWhileReadingTemplate, result.Message);
        }

        [Fact]
        public void ReadAttendance_AttendanceSheetIsNotValid_ReturnFailureResult()
        {
            // Arrange
            var fakeCell = Substitute.For<ICell>();
            fakeCell.StringCellValue.Returns("aaa");
            fakeCell.ColumnIndex.Returns(2);
            var mockCellList = new List<ICell> { fakeCell };

            var fakeRow = Substitute.For<IRow>();
            fakeRow.Cells.Returns(mockCellList);

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceHeaderRow)).Returns(fakeRow);

            var template = new AttendanceTemplate
            {
                Sheet = fakeSheet
            };

            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(template);

            // Assert
            var result = Assert.IsType<Failure<AttendanceTemplate>>(actual);
            Assert.Equal(Message.IncorrectTemplate, result.Message);
        }

        [Fact]
        public void ReadAttendance_SheetIsValidAndAllRowsIsNotValid_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeAttendanceHeaderRow();
            var fakeEmptyRow = Substitute.For<IRow>();
            fakeEmptyRow.Cells.Returns(new List<ICell>());

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(6);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceStartRow)).Returns(fakeEmptyRow);

            var template = new AttendanceTemplate
            {
                Sheet = fakeSheet
            };

            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(template);

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            Assert.Empty(result.Value.Records);
            Assert.Empty(result.Value.ValidationResults);
        }

        [Fact]
        public void ReadAttendance_EmployeeCodeIsEmpty_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeAttendanceHeaderRow();
            var fakeRow = FakeRow(5, "", "0");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(6);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceStartRow)).Returns(fakeRow);

            var template = new AttendanceTemplate
            {
                Sheet = fakeSheet
            };
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(template);

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            Assert.Single(result.Value.Records);
            Assert.Equal(5 + 1, result.Value.Records.First().RowNumber);
            Assert.Equal("", result.Value.Records.First().EmployeeCode);
            Assert.Equal("0", result.Value.Records.First().ActualWorkingDay);
            Assert.Single(result.Value.ValidationResults);
            Assert.Equal(5 + 1, result.Value.ValidationResults.First().RowNumber);
            Assert.Equal(string.Format(Message.ValueMissing, AttendanceColumn.IDNo), result.Value.ValidationResults.First().Message);
            Assert.Equal(2, result.Value.ValidationResults.First().ResultType);
        }

        [Fact]
        public void ReadAttendance_EmployeeCodeNotExistInCompanyAndActualWorkingDayIsEmpty_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeAttendanceHeaderRow();
            var fakeRow = FakeRow(5, "0224", "");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(6);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceStartRow)).Returns(fakeRow);

            var template = new AttendanceTemplate
            {
                Sheet = fakeSheet,
                CompanyCode = "DAR",
                EmployeeCodeList = new List<string> { "111" }
            };
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(template);

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            Assert.Single(result.Value.Records);
            Assert.Equal(5 + 1, result.Value.Records.First().RowNumber);
            Assert.Equal("0224", result.Value.Records.First().EmployeeCode);
            Assert.Equal("", result.Value.Records.First().ActualWorkingDay);
            var validationResults = result.Value.ValidationResults.ToList();
            Assert.Equal(5 + 1, validationResults[0].RowNumber);
            Assert.Equal(string.Format(Message.IDNoNotExistIn, template.CompanyCode), validationResults[0].Message);
            Assert.Equal(2, validationResults[0].ResultType);
            Assert.Equal(5 + 1, validationResults[1].RowNumber);
            Assert.Equal(string.Format(Message.ValueMissing, AttendanceColumn.ActualWorkingDay), validationResults[1].Message);
            Assert.Equal(2, validationResults[1].ResultType);
        }

        [Fact]
        public void ReadAttendance_EmployeeCodeIsDuplicatedAndActualWorkingDayIsNotANumber_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeAttendanceHeaderRow();
            var fakeRow1 = FakeRow(5, "0224", "1");
            var fakeRow2 = FakeRow(6, "0224", "xxx");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(7);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceStartRow)).Returns(fakeRow1);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceStartRow + 1)).Returns(fakeRow2);

            var template = new AttendanceTemplate
            {
                Sheet = fakeSheet,
                CompanyCode = "DAR",
                EmployeeCodeList = new List<string> { "0224" }
            };
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(template);

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            var records = result.Value.Records.ToList();
            Assert.Equal(5 + 1, records[0].RowNumber);
            Assert.Equal("0224", records[0].EmployeeCode);
            Assert.Equal("1", records[0].ActualWorkingDay);
            Assert.Equal(6 + 1, records[1].RowNumber);
            Assert.Equal("0224", records[1].EmployeeCode);
            Assert.Equal("xxx", records[1].ActualWorkingDay);
            var validationResults = result.Value.ValidationResults.ToList();
            Assert.Equal(6 + 1, validationResults[0].RowNumber);
            Assert.Equal(Message.IDNoIsDuplicated, validationResults[0].Message);
            Assert.Equal(2, validationResults[0].ResultType);
            Assert.Equal(6 + 1, validationResults[1].RowNumber);
            Assert.Equal(string.Format(Message.ValueMustBeANumber, AttendanceColumn.ActualWorkingDay), validationResults[1].Message);
            Assert.Equal(2, validationResults[1].ResultType);
        }

        [Fact]
        public void ReadAttendance_ActualWorkingDayIsGreaterThan31_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeAttendanceHeaderRow();
            var fakeRow = FakeRow(5, "0224", "33");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(6);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == AttendanceStartRow)).Returns(fakeRow);

            var template = new AttendanceTemplate
            {
                Sheet = fakeSheet,
                CompanyCode = "DAR",
                EmployeeCodeList = new List<string> { "0224" }
            };
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(template);

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            var records = result.Value.Records.ToList();
            Assert.Equal(5 + 1, records[0].RowNumber);
            Assert.Equal("0224", records[0].EmployeeCode);
            Assert.Equal("33", records[0].ActualWorkingDay);
            var validationResults = result.Value.ValidationResults.ToList();
            Assert.Equal(5 + 1, validationResults[0].RowNumber);
            Assert.Equal(string.Format(Message.ValueMustBeGreaterAndNotExceed, AttendanceColumn.ActualWorkingDay, 31), validationResults[0].Message);
            Assert.Equal(2, validationResults[0].ResultType);
        }

        #endregion ReadAttendance

        #region ReadDSTNetSalary

        [Fact]
        public void ReadDSTNetSalary_WhenThrowException_ReturnFailureResult()
        {
            // Arrange
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadDSTNetSalary(new DSTNetSalaryTemplate());

            // Assert
            var result = Assert.IsType<Failure<DSTNetSalaryTemplate>>(actual);
            Assert.Equal(Message.ErrorOccuredWhileReadingTemplate, result.Message);
        }

        [Fact]
        public void ReadDSTNetSalary_DSTNetSalarySheetIsNotValid_ReturnFailureResult()
        {
            // Arrange
            var fakeCell = Substitute.For<ICell>();
            fakeCell.StringCellValue.Returns("aaa");
            fakeCell.ColumnIndex.Returns(1);
            var mockCellList = new List<ICell> { fakeCell };

            var fakeRow = Substitute.For<IRow>();
            fakeRow.Cells.Returns(mockCellList);

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryHeaderRow)).Returns(fakeRow);

            var template = new DSTNetSalaryTemplate
            {
                Sheet = fakeSheet
            };

            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadDSTNetSalary(template);

            // Assert
            var result = Assert.IsType<Failure<DSTNetSalaryTemplate>>(actual);
            Assert.Equal(Message.IncorrectTemplate, result.Message);
        }

        [Fact]
        public void ReadDSTNetSalary_SheetIsValidAndAllRowsIsNotValid_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeDSTNetSalaryHeaderRow();
            var fakeEmptyRow = Substitute.For<IRow>();
            fakeEmptyRow.Cells.Returns(new List<ICell>());

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(5);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryStartRow)).Returns(fakeEmptyRow);

            var template = new DSTNetSalaryTemplate
            {
                Sheet = fakeSheet
            };

            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadDSTNetSalary(template);

            // Assert
            var result = Assert.IsType<Success<DSTNetSalaryTemplate>>(actual);
            Assert.Empty(result.Value.Records);
            Assert.Empty(result.Value.ValidationResults);
        }

        [Fact]
        public void ReadDSTNetSalary_CurrencyIsEmptyAndSalaryIsEmpty_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeDSTNetSalaryHeaderRow();
            var fakeRow = FakeRow(5, "0224", "", "");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(5);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryStartRow)).Returns(fakeRow);

            var template = new DSTNetSalaryTemplate
            {
                Sheet = fakeSheet,
                CompanyCode = "DAR",
                EmployeeCodeList = new List<string> { "0224" }
            };
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadDSTNetSalary(template);

            // Assert
            var result = Assert.IsType<Success<DSTNetSalaryTemplate>>(actual);
            Assert.Single(result.Value.Records);
            Assert.Equal(5 + 1, result.Value.Records.First().RowNumber);
            Assert.Equal("0224", result.Value.Records.First().EmployeeCode);
            Assert.Equal("", result.Value.Records.First().Currency);
            Assert.Equal("", result.Value.Records.First().NetSalaryReceived);

            var validationResults = result.Value.ValidationResults.ToList();
            Assert.Equal(5 + 1, validationResults[0].RowNumber);
            Assert.Equal(string.Format(Message.ValueMissing, DSTNetSalaryColumn.Currency), validationResults[0].Message);
            Assert.Equal(2, validationResults[0].ResultType);

            Assert.Equal(5 + 1, validationResults[1].RowNumber);
            Assert.Equal(string.Format(Message.ValueMissing, DSTNetSalaryColumn.NetSalaryReceived), validationResults[1].Message);
            Assert.Equal(2, validationResults[1].ResultType);
        }

        [Fact]
        public void ReadDSTNetSalary_CurrencyNotExistsAndSalaryNotANumber_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeDSTNetSalaryHeaderRow();
            var fakeRow = FakeRow(5, "0224", "cuu", "qwer");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(5);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryStartRow)).Returns(fakeRow);

            var template = new DSTNetSalaryTemplate
            {
                Sheet = fakeSheet,
                CompanyCode = "DAR",
                EmployeeCodeList = new List<string> { "0224" },
                Currencies = new List<Currency> { new Currency { CurrencyCode = "USD" } }
            };
            var templateReader = GetTemplateReader();
            cryptoService.Encrypt("qwer").Returns("Encryptqwer");

            // Act
            var actual = templateReader.ReadDSTNetSalary(template);

            // Assert
            var result = Assert.IsType<Success<DSTNetSalaryTemplate>>(actual);
            Assert.Single(result.Value.Records);
            Assert.Equal(5 + 1, result.Value.Records.First().RowNumber);
            Assert.Equal("0224", result.Value.Records.First().EmployeeCode);
            Assert.Equal("cuu", result.Value.Records.First().Currency);
            Assert.Equal("Encryptqwer", result.Value.Records.First().NetSalaryReceived);

            var validationResults = result.Value.ValidationResults.ToList();
            Assert.Equal(5 + 1, validationResults[0].RowNumber);
            Assert.Equal(Message.CurrencyNotExists, validationResults[0].Message);
            Assert.Equal(2, validationResults[0].ResultType);

            Assert.Equal(5 + 1, validationResults[1].RowNumber);
            Assert.Equal(string.Format(Message.ValueMustBeANumber, DSTNetSalaryColumn.NetSalaryReceived), validationResults[1].Message);
            Assert.Equal(2, validationResults[1].ResultType);
        }

        [Fact]
        public void ReadDSTNetSalary_EmployeeCodeIsEmpty_ReturnSuccessResultWithExpectedValue()
        {
            // Arrange
            var fakeHeaderRow = FakeDSTNetSalaryHeaderRow();
            var fakeRow = FakeRow(5, "", "usd", "1200");

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(5);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryHeaderRow)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == DSTNetSalaryStartRow)).Returns(fakeRow);

            var template = new DSTNetSalaryTemplate
            {
                Sheet = fakeSheet,
                CompanyCode = "DAR",
                EmployeeCodeList = new List<string> { "0224" },
                Currencies = new List<Currency> { new Currency { CurrencyCode = "USD" } }
            };
            var templateReader = GetTemplateReader();
            cryptoService.Encrypt("1200").Returns("Encrypt1200");

            // Act
            var actual = templateReader.ReadDSTNetSalary(template);

            // Assert
            var result = Assert.IsType<Success<DSTNetSalaryTemplate>>(actual);
            Assert.Single(result.Value.Records);
            Assert.Equal(5 + 1, result.Value.Records.First().RowNumber);
            Assert.Equal("", result.Value.Records.First().EmployeeCode);
            Assert.Equal("usd", result.Value.Records.First().Currency);
            Assert.Equal("Encrypt1200", result.Value.Records.First().NetSalaryReceived);

            var validationResults = result.Value.ValidationResults.ToList();
            Assert.Equal(5 + 1, validationResults[0].RowNumber);
            Assert.Equal(string.Format(Message.ValueMissing, DSTNetSalaryColumn.IDNo), validationResults[0].Message);
            Assert.Equal(2, validationResults[0].ResultType);
        }

        #endregion ReadDSTNetSalary

        private ITemplateReader GetTemplateReader()
        {
            cryptoService = FakeCryptoService();
            var reader = new TemplateReader(cryptoService);

            return reader;
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static IRow FakeAttendanceHeaderRow()
        {
            var fakeCellIDNo = Substitute.For<ICell>();
            fakeCellIDNo.ColumnIndex.Returns(1);
            fakeCellIDNo.StringCellValue.Returns(AttendanceColumn.IDNo);

            var fakeCellActualWorkingDay = Substitute.For<ICell>();
            fakeCellActualWorkingDay.ColumnIndex.Returns(2);
            fakeCellActualWorkingDay.StringCellValue.Returns(AttendanceColumn.ActualWorkingDay);

            var mockCellList = new List<ICell> { fakeCellIDNo, fakeCellActualWorkingDay };

            var fakeHeaderRow = Substitute.For<IRow>();
            fakeHeaderRow.Cells.Returns(mockCellList);

            return fakeHeaderRow;
        }

        private static IRow FakeRow(int rowIndex, string firstCellValue, string secondCellValue, string thirdCellValue = "")
        {
            var firstRichText = Substitute.For<IRichTextString>();
            firstRichText.String.Returns(firstCellValue);

            var secondRichText = Substitute.For<IRichTextString>();
            secondRichText.String.Returns(secondCellValue);

            var thirdRichText = Substitute.For<IRichTextString>();
            thirdRichText.String.Returns(thirdCellValue);

            var firstCell = Substitute.For<ICell>();
            firstCell.ColumnIndex.Returns(1);
            firstCell.StringCellValue.Returns(firstCellValue);
            firstCell.CellType.Returns(CellType.String);
            firstCell.RowIndex.Returns(rowIndex);
            firstCell.RichStringCellValue.Returns(firstRichText);

            var secondCell = Substitute.For<ICell>();
            secondCell.ColumnIndex.Returns(2);
            secondCell.StringCellValue.Returns(secondCellValue);
            secondCell.CellType.Returns(CellType.String);
            secondCell.RowIndex.Returns(rowIndex);
            secondCell.RichStringCellValue.Returns(secondRichText);

            var thirdCell = Substitute.For<ICell>();
            thirdCell.ColumnIndex.Returns(2);
            thirdCell.StringCellValue.Returns(secondCellValue);
            thirdCell.CellType.Returns(CellType.String);
            thirdCell.RowIndex.Returns(rowIndex);
            thirdCell.RichStringCellValue.Returns(thirdRichText);

            var mockCellList = new List<ICell> { firstCell, secondCell, thirdCell };
            var fakeRow = Substitute.For<IRow>();
            fakeRow.Cells.Returns(mockCellList);
            fakeRow.GetCell(1).Returns(firstCell);
            fakeRow.GetCell(2).Returns(secondCell);
            fakeRow.GetCell(3).Returns(thirdCell);
            fakeRow.RowNum.Returns(rowIndex);

            return fakeRow;
        }

        private static IRow FakeDSTNetSalaryHeaderRow()
        {
            var fakeCellIDNo = Substitute.For<ICell>();
            fakeCellIDNo.ColumnIndex.Returns(1);
            fakeCellIDNo.StringCellValue.Returns(DSTNetSalaryColumn.IDNo);

            var fakeCellCurrency = Substitute.For<ICell>();
            fakeCellCurrency.ColumnIndex.Returns(2);
            fakeCellCurrency.StringCellValue.Returns(DSTNetSalaryColumn.Currency);

            var fakeCellSalary = Substitute.For<ICell>();
            fakeCellSalary.ColumnIndex.Returns(3);
            fakeCellSalary.StringCellValue.Returns(DSTNetSalaryColumn.NetSalaryReceived);

            var mockCellList = new List<ICell> { fakeCellIDNo, fakeCellCurrency, fakeCellSalary };

            var fakeHeaderRow = Substitute.For<IRow>();
            fakeHeaderRow.Cells.Returns(mockCellList);

            return fakeHeaderRow;
        }
    }
}