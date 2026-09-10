using System.ComponentModel.DataAnnotations;

namespace RepairRequest.Web.ViewModels.Account;

public class RegisterViewModel
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "Username may contain letters, numbers, periods, underscores, and hyphens only.")]
    [Display(Name = "Username")]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; init; } = string.Empty;
}
