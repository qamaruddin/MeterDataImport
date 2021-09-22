using CsvHelper;
using MeterDataImport.Api.RequestDto;
using MeterDataImport.Api.ResponseDto;
using MeterDataImport.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MeterDataImport.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeterReadingController : ControllerBase
    {
        private readonly IFileImporterService _fileImporterService;

        public MeterReadingController(IFileImporterService fileImporterService)
        {
            _fileImporterService = fileImporterService;
        }

        [HttpPost("meter-reading-uploads")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(FileImportResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportMeterData(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("File must be provided.");
            }

            if (file.FileName.Split('.')[file.FileName.Split('.').Length - 1] != "csv")
            {
                return BadRequest("Only csv files supported.");
            }

            var fileImportRequest = new List<FileImportRequest>();
            using (var sreader = new StreamReader(file.OpenReadStream()))
            {
                using var csvReader = new CsvReader(sreader, CultureInfo.InvariantCulture);
                fileImportRequest = csvReader.GetRecords<FileImportRequest>().ToList();
            }

            var importResult = await _fileImporterService.ImportMeterData(fileImportRequest);

            return Ok(importResult);
        }
    }
}
