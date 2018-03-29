namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using HRP.Core.Constants;
    using HRP.Core.Extensions;
    using HRP.Core.Services;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using NPOI.SS.UserModel;
    using NSubstitute;
    using Xunit;

    public class TemplateReaderTests
    {
        private ICryptoService cryptoService;

        [Fact]
        public void ReadAttendance_WhenThrowException_ReturnFailureResult()
        {
            // Arrange
            var templateReader = GetTemplateReader();

            // Act
            var actual = templateReader.ReadAttendance(new AttendanceTemplate());

            // Assert
            var result = Assert.IsType<Failure<AttendanceTemplate>>(actual);
            Assert.Equal(Message.IncorrectTemplate, result.Message);
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
            fakeSheet.GetRow(Arg.Is<int>(x => x == 5)).Returns(fakeRow);

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
            var fakeHeaderRow = FakeHeaderRow();
            var fakeEmptyRow = Substitute.For<IRow>();
            fakeEmptyRow.Cells.Returns(new List<ICell>());

            var fakeSheet = Substitute.For<ISheet>();
            fakeSheet.LastRowNum.Returns(6);
            fakeSheet.GetRow(Arg.Is<int>(x => x == 5)).Returns(fakeHeaderRow);
            fakeSheet.GetRow(Arg.Is<int>(x => x == 6)).Returns(fakeEmptyRow);

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

        //[Fact]
        //public void ReadAttendance_SheetIsValidAndRowsIsValid_ReturnSuccessResultWithExpectedValue()
        //{
        //    // Arrange
        //    var fakeHeaderRow = FakeHeaderRow();
        //    var fakeValidRow = FakeValidRow();

        //    var fakeSheet = Substitute.For<ISheet>();
        //    fakeSheet.LastRowNum.Returns(6);
        //    fakeSheet.GetRow(Arg.Is<int>(x => x == 5)).Returns(fakeHeaderRow);
        //    fakeSheet.GetRow(Arg.Is<int>(x => x == 6)).Returns(fakeValidRow);

        //    var template = new AttendanceTemplate
        //    {
        //        Sheet = fakeSheet
        //    };

        //    var templateReader = GetTemplateReader();

        //    // Act
        //    var actual = templateReader.ReadAttendance(template);

        //    // Assert
        //    var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
        //    Assert.Empty(result.Value.Records);
        //    Assert.Empty(result.Value.ValidationResults);
        //}

        private ITemplateReader GetTemplateReader()
        {
            cryptoService = FakeCryptoService();

            var reader = new TemplateReader(cryptoService);
            return reader;
        }

        private ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private IRow FakeHeaderRow()
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

        private IRow FakeValidRow()
        {
            var fakeCellIDNo = Substitute.For<ICell>();
            fakeCellIDNo.ColumnIndex.Returns(1);
            fakeCellIDNo.StringCellValue.Returns("02248");
            var fakeCellActualWorkingDay = Substitute.For<ICell>();
            fakeCellActualWorkingDay.ColumnIndex.Returns(2);
            fakeCellActualWorkingDay.StringCellValue.Returns("23.5");

            var mockCellList = new List<ICell> { fakeCellIDNo, fakeCellActualWorkingDay };

            var fakeRow = Substitute.For<IRow>();
            fakeRow.Cells.Returns(mockCellList);
            fakeRow.GetCell(1).Returns(fakeCellIDNo);
            fakeRow.GetCell(2).Returns(fakeCellActualWorkingDay);
            return fakeRow;
        }
    }
}