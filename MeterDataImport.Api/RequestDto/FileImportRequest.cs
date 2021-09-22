using CsvHelper.Configuration.Attributes;
using System;

namespace MeterDataImport.Api.RequestDto
{
    public class FileImportRequest
    {
        public int AccountId { get; set; }
        
        [Format("dd/MM/yyyy HH:mm")]
        public DateTime MeterReadingDateTime { get; set; }
        
        public string MeterReadValue { get; set; }
    }
}
