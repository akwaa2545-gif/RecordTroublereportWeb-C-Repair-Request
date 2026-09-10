using System.ComponentModel.DataAnnotations;
using RepairRequest.Web.Models;
using RepairRequest.Web.ViewModels.RepairRequests;

namespace RepairRequest.Web.Tests.ViewModels;

public class CreateRepairRequestViewModelTests
{
    [Fact]
    public void Valid_model_has_no_validation_errors()
    {
        var model = new CreateRepairRequestViewModel
        {
            EquipmentCode = "EQ-101",
            Location = "Line 1",
            Title = "Machine will not start",
            Description = "The machine is not starting.",
            Priority = RepairPriority.Normal
        };

        var validationResults = Validate(model);

        Assert.Empty(validationResults);
    }

    [Fact]
    public void Missing_required_fields_has_validation_errors()
    {
        var validationResults = Validate(new CreateRepairRequestViewModel());

        Assert.Equal(4, validationResults.Count);
    }

    private static List<ValidationResult> Validate(CreateRepairRequestViewModel model)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), validationResults, validateAllProperties: true);
        return validationResults;
    }
}
