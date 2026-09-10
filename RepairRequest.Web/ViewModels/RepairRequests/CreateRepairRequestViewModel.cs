using System.ComponentModel.DataAnnotations;
using RepairRequest.Web.Models;
using RepairRequestEntity = global::RepairRequest.Web.Models.RepairRequest;

namespace RepairRequest.Web.ViewModels.RepairRequests;

public class CreateRepairRequestViewModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Equipment code")]
    public string EquipmentCode { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Location { get; init; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(RepairRequestEntity.MaximumDescriptionLength)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [EnumDataType(typeof(RepairPriority))]
    public RepairPriority Priority { get; init; } = RepairPriority.Normal;
}
