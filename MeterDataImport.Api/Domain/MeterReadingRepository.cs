using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MeterDataImport.Api.Domain
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly MeterDataImportApDbContext _dbContext;

        public MeterReadingRepository(MeterDataImportApDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MeterReading>> GetMeterReadingByFilter(Expression<Func<MeterReading, bool>> whereExpression)
        {
            return await _dbContext.MeterReadings.Where(whereExpression).ToListAsync();
        }

        public async Task<int> SaveMeterReading(MeterReading meterReading)
        {
            await _dbContext.MeterReadings.AddRangeAsync(meterReading);
            var result = await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
