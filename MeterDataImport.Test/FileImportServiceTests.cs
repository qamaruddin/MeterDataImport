using MeterDataImport.Api.Domain;
using MeterDataImport.Api.RequestDto;
using MeterDataImport.Api.Services;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace MeterDataImport.Test
{
    public class FileImportServiceTests
    {
        [Fact]
        public void GivenListOfReading_WhenImportIsCalled_ShouldExcludeInvalidData()
        {
            var meterReadingRepo = new Mock<IMeterReadingRepository>();
            meterReadingRepo.Setup(x => x.GetMeterReadingByFilter(It.IsAny<Expression<Func<MeterReading, bool>>>()).Result).Returns(new List<MeterReading> { });
            meterReadingRepo.Setup(x => x.SaveMeterReading(It.IsAny<MeterReading>()).Result).Returns(1);
            var fileImportRequests = new List<FileImportRequest>
            {
                new FileImportRequest {AccountId = 01002, MeterReadingDateTime = DateTime.MinValue, MeterReadValue = "01002" },
                new FileImportRequest {AccountId = 1002, MeterReadingDateTime = DateTime.Now, MeterReadValue = "VOID" },
                new FileImportRequest {AccountId = 1003, MeterReadingDateTime = DateTime.MinValue, MeterReadValue = "999999" }
            };

            var fileImportService = new FileImporterService(meterReadingRepo.Object, new FileImportRequestValidator());
            var result = fileImportService.ImportMeterData(fileImportRequests).Result;

            result.Succeded.ShouldBe(0);
            result.Failed.ShouldBe(3);
            meterReadingRepo.Verify(x => x.GetMeterReadingByFilter(It.IsAny<Expression<Func<MeterReading, bool>>>()), Times.Never);
            meterReadingRepo.Verify(x => x.SaveMeterReading(It.IsAny<MeterReading>()), Times.Never);
        }

        [Fact]
        public void GivenListOfReading_WhenImportIsCalled_ShouldExcludeNonExistentAccountData()
        {
            var currentDt = DateTime.Now;
            var meterReadingRepo = new Mock<IMeterReadingRepository>();
            meterReadingRepo.Setup(x => x.GetMeterReadingByFilter(It.IsAny<Expression<Func<MeterReading, bool>>>()).Result)
                .Returns(new List<MeterReading> 
                { 
                    new MeterReading { AccountId = 1234, ReadingDateTime = currentDt, Value = 01002}
                });
            meterReadingRepo.Setup(x => x.SaveMeterReading(It.IsAny<MeterReading>()).Result).Returns(1);
            var fileImportRequests = new List<FileImportRequest>
            {
                new FileImportRequest {AccountId = 1234, MeterReadingDateTime = currentDt, MeterReadValue = "01002" },
                new FileImportRequest {AccountId = 1002, MeterReadingDateTime = DateTime.Now, MeterReadValue = "VOID" },
                new FileImportRequest {AccountId = 1003, MeterReadingDateTime = DateTime.MinValue, MeterReadValue = "999999" }
            };

            var fileImportService = new FileImporterService(meterReadingRepo.Object, new FileImportRequestValidator());
            var result = fileImportService.ImportMeterData(fileImportRequests).Result;

            result.Succeded.ShouldBe(0);
            result.Failed.ShouldBe(3);
            meterReadingRepo.Verify(x => x.GetMeterReadingByFilter(It.IsAny<Expression<Func<MeterReading, bool>>>()), Times.Once);
            meterReadingRepo.Verify(x => x.SaveMeterReading(It.IsAny<MeterReading>()), Times.Never);
        }

        [Fact]
        public void GivenListOfReading_WhenImportIsCalled_ShouldNotImportDataAlreadyExists()
        {
            var currentDt = DateTime.Now;
            var meterReadingRepo = new Mock<IMeterReadingRepository>();
            meterReadingRepo.Setup(x => x.GetMeterReadingByFilter(It.IsAny<Expression<Func<MeterReading, bool>>>()).Result)
                .Returns(new List<MeterReading>
                {
                    new MeterReading { AccountId = 1234, ReadingDateTime = currentDt, Value = 01002}
                });
            meterReadingRepo.Setup(x => x.SaveMeterReading(It.IsAny<MeterReading>()).Result).Returns(1);
            var fileImportRequests = new List<FileImportRequest>
            {
                new FileImportRequest {AccountId = 1234, MeterReadingDateTime = currentDt, MeterReadValue = "01002" },
                new FileImportRequest {AccountId = 1234, MeterReadingDateTime = currentDt.AddMinutes(30), MeterReadValue = "01000" },
                new FileImportRequest {AccountId = 1235, MeterReadingDateTime = currentDt, MeterReadValue = "06566" },
                new FileImportRequest {AccountId = 1236, MeterReadingDateTime = DateTime.MinValue, MeterReadValue = "999999" }
            };

            var fileImportService = new FileImporterService(meterReadingRepo.Object, new FileImportRequestValidator());
            var result = fileImportService.ImportMeterData(fileImportRequests).Result;

            result.Succeded.ShouldBe(2);
            result.Failed.ShouldBe(2);
            meterReadingRepo.Verify(x => x.GetMeterReadingByFilter(It.IsAny<Expression<Func<MeterReading, bool>>>()), Times.Once);
            meterReadingRepo.Verify(x => x.SaveMeterReading(It.Is<MeterReading>(x => x.AccountId == 1234)), Times.Once);
            meterReadingRepo.Verify(x => x.SaveMeterReading(It.Is<MeterReading>(x => x.AccountId == 1235)), Times.Once);
        }
    }

}
