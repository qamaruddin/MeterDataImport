using FluentValidation;
using MeterDataImport.Api.Domain;
using MeterDataImport.Api.RequestDto;
using MeterDataImport.Api.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterDataImport.Api.Services
{
    public class FileImporterService : IFileImporterService
    {
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IValidator<FileImportRequest> _validator;

        public FileImporterService(IMeterReadingRepository meterReadingRepository, IValidator<FileImportRequest> validator)
        {
            _meterReadingRepository = meterReadingRepository;
            _validator = validator;
        }

        public async Task<FileImportResponse> ImportMeterData(IEnumerable<FileImportRequest> fileImportRequests)
        {
            //sanitize the invalid data.
            var validFileImportRequest = new List<FileImportRequest>();
            var invalidFileImportRequestCount = 0;
            var validFileImportRequestCount = 0;
            foreach (var item in fileImportRequests)
            {
                var validationResult = await _validator.ValidateAsync(item);
                if (!validationResult.IsValid)
                {
                    invalidFileImportRequestCount++;
                    continue;
                }
                validFileImportRequest.Add(item);
            }

            if (!validFileImportRequest.Any())
            {
                return new FileImportResponse 
                {
                    Succeded = validFileImportRequestCount,
                    Failed = invalidFileImportRequestCount
                };
            }

            //get existing reading data
            var accountIds = validFileImportRequest.Select(x => x.AccountId).Distinct();
            var minDateOfImportReq = fileImportRequests.Min(x => x.MeterReadingDateTime);
            var maxDateOfImportReq = fileImportRequests.Max(x => x.MeterReadingDateTime);
            var existingMeterReading = await _meterReadingRepository.GetMeterReadingByFilter(
                x => accountIds.Contains(x.AccountId)
                && x.ReadingDateTime >= minDateOfImportReq
                && x.ReadingDateTime <= maxDateOfImportReq);

            //match existing data contains any new reading 
            var groupedImportData = validFileImportRequest
                .GroupBy(x => new { x.AccountId, x.MeterReadingDateTime })
                .Select(x => new
                {
                    AccountId = x.Key.AccountId,
                    MeterReadingDateTime = x.Key.MeterReadingDateTime,
                    LatestReading = x.FirstOrDefault()
                });

            foreach (var item in groupedImportData)
            {
                var readingExists = existingMeterReading.Any(x => x.AccountId == item.AccountId && x.ReadingDateTime == item.MeterReadingDateTime);
                if (!readingExists)
                {
                    continue;
                }
                validFileImportRequest.RemoveAll(x => x.AccountId == item.AccountId && x.MeterReadingDateTime == item.MeterReadingDateTime);
                invalidFileImportRequestCount++;
            }
            
            foreach (var item in validFileImportRequest)
            {
                try
                {
                    if (!int.TryParse(item.MeterReadValue, out int parsedReadValue))
                    {
                        invalidFileImportRequestCount++;
                        continue;
                    }
                    await _meterReadingRepository.SaveMeterReading(new MeterReading
                    {
                        AccountId = item.AccountId,
                        ReadingDateTime = item.MeterReadingDateTime,
                        Value = parsedReadValue
                    });
                    validFileImportRequestCount++;
                }
                catch (Exception)
                {
                    invalidFileImportRequestCount++;
                }
            }
            
            return new FileImportResponse 
            {
                Failed = invalidFileImportRequestCount,
                Succeded = validFileImportRequestCount
            };
        }
    }
}
