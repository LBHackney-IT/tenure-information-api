using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Infrastructure.Exceptions
{
    public class EditTenureInformationValidationException : Exception
    {
        public ValidationResult ValidationResult { get; private set; }

        public EditTenureInformationValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
    }
}
