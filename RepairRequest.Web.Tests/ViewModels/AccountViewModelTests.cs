using System.ComponentModel.DataAnnotations;
using RepairRequest.Web.ViewModels.Account;

namespace RepairRequest.Web.Tests.ViewModels;

public class AccountViewModelTests
{
    [Fact]
    public void Register_model_with_valid_username_and_matching_passwords_has_no_errors()
    {
        var model = new RegisterViewModel
        {
            UserName = "operator.1",
            Password = "Repair!2026",
            ConfirmPassword = "Repair!2026"
        };

        Assert.Empty(Validate(model));
    }

    [Fact]
    public void Register_model_rejects_mismatched_passwords()
    {
        var model = new RegisterViewModel
        {
            UserName = "operator.1",
            Password = "Repair!2026",
            ConfirmPassword = "Different!2026"
        };

        Assert.NotEmpty(Validate(model));
    }

    [Fact]
    public void Login_model_requires_username_and_password()
    {
        Assert.Equal(2, Validate(new LoginViewModel()).Count);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), validationResults, validateAllProperties: true);
        return validationResults;
    }
}
