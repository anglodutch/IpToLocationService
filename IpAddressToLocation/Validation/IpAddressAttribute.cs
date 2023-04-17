using System.ComponentModel.DataAnnotations;
using System.Net;

public class IpAddressAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
        IPAddress.TryParse(value?.ToString(), out _)
            ? ValidationResult.Success
            : new ValidationResult("IP Address is invalid");
}