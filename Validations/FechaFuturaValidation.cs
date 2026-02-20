using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.Validations
{
    public class FechaFuturaValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value == null || (value is DateTime fecha && fecha > DateTime.Now))
                return ValidationResult.Success;
            return new ValidationResult("La fecha y hora tiene que ser posterior a la actual");
        }
    }
}
