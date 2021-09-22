using MeterDataImport.Api.RequestDto;
using MeterDataImport.Api.ResponseDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeterDataImport.Api.Services
{
    public interface IFileImporterService
    {
        Task<FileImportResponse> ImportMeterData(IEnumerable<FileImportRequest> fileImportRequests);
    }
}
