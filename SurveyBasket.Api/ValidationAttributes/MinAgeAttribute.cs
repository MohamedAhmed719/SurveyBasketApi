using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Api.ValidationAttributes;

[AttributeUsage(AttributeTargets.All)]
public class MinAgeAttribute(int minAge) : ValidationAttribute
{
    private readonly int _minAge = minAge;

    protected override ValidationResult? IsValid(object? value,ValidationContext validationContext)
    {
        if(value is not null)
        {
            var date = (DateTime)value;

            if (DateTime.Today < date.AddYears(_minAge))
                return new ValidationResult($"Invalid {validationContext.DisplayName} age should be {_minAge}");

        }


        return ValidationResult.Success;
        
    }
}
