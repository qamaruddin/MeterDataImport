using FluentValidation;
using System;

namespace MeterDataImport.Api.RequestDto
{
    public class FileImportRequestValidator : AbstractValidator<FileImportRequest>
    {
        public FileImportRequestValidator()
        {
            RuleFor(x => x.AccountId)
                .GreaterThan(0);
            RuleFor(x => x.MeterReadingDateTime)
                .NotNull()
                .Must(x => x != DateTime.MinValue);
            RuleFor(x => x.MeterReadValue)
                .Length(5, 5)
                .Must(x =>
                {
                    if (!int.TryParse(x, out int result))
                    {
                        return false;
                    }
                    return true;
                });
        }
    }
}
