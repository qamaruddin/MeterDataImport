using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MeterDataImport.Api.Domain
{
    public interface IMeterReadingRepository
    {
        Task<IEnumerable<MeterReading>> GetMeterReadingByFilter(Expression<Func<MeterReading, bool>> whereExpression);
        Task<int> SaveMeterReading(MeterReading meterReading);
    }
}
