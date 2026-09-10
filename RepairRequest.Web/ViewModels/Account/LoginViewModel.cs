using System.ComponentModel.DataAnnotations;

namespace RepairRequest.Web.ViewModels.Account;

public class LoginViewModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Username")]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; init; }
}
