using CsvHelper;
using MeterDataImport.Api.Controllers;
using MeterDataImport.Api.RequestDto;
using MeterDataImport.Api.ResponseDto;
using MeterDataImport.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;

namespace MeterDataImport.Test
{
    public class MeterReadingControllerTests
    {
        [Fact]
        public async void GivenNoFile_WhenImportIsCalled_ShouldReturnBadRequest()
        {
            var importService = new Mock<IFileImporterService>();
            importService.Setup(x => x.ImportMeterData(It.IsAny<IEnumerable<FileImportRequest>>()).Result).Returns(new FileImportResponse { Succeded = 10, Failed = 10 });

            var controller = new MeterReadingController(importService.Object);
            var result = await controller.ImportMeterData(null) as ObjectResult;

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBe("File must be provided.");
        }

        [Fact]
        public async void GivenUnsupportedFileType_WhenImportIsCalled_ShouldReturnBadRequest()
        {
            var importService = new Mock<IFileImporterService>();
            importService.Setup(x => x.ImportMeterData(It.IsAny<IEnumerable<FileImportRequest>>()).Result).Returns(new FileImportResponse { Succeded = 0, Failed = 10 });

            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, "filedata", fileName);

            var controller = new MeterReadingController(importService.Object);
            var result = await controller.ImportMeterData(file) as ObjectResult;

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBe("Only csv files supported.");
        }

        [Fact]
        public async void GivenValidCsvFile_WhenImportIsCalled_ShouldReturnOk()
        {
            var importService = new Mock<IFileImporterService>();
            importService.Setup(x => x.ImportMeterData(It.IsAny<IEnumerable<FileImportRequest>>()).Result).Returns(new FileImportResponse { Succeded = 2, Failed = 0 });

            var fakeFile = GenerateFakeFile();
            MemoryStream fileMs = new MemoryStream(fakeFile);
            IFormFile file = new FormFile(fileMs, 0, fakeFile.Length, "filedata", "meter_data.csv");
            
            var controller = new MeterReadingController(importService.Object);
            var result = await controller.ImportMeterData(file) as ObjectResult;
            
            result.StatusCode.ShouldBe(200);
            
            fileMs.Close();
        }

        private byte[] GenerateFakeFile()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(new List<FileImportRequest>
                    {
                        new FileImportRequest { AccountId = 1001, MeterReadingDateTime = DateTime.Now, MeterReadValue = "01000"},
                        new FileImportRequest { AccountId = 1002, MeterReadingDateTime = DateTime.Now, MeterReadValue = "01000"}
                    });
                }

                return memoryStream.ToArray();
            }
        }
    }
}
