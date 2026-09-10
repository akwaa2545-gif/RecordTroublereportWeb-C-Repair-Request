namespace RepairRequest.Web.Models;

public class RepairRequest
{
    public const int MaximumDescriptionLength = 2000;

    private RepairRequest()
    {
    }

    public Guid Id { get; private set; }
    public string RequestNumber { get; private set; } = string.Empty;
    public string RequesterId { get; private set; } = string.Empty;
    public string EquipmentCode { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public RepairPriority Priority { get; private set; }
    public RepairStatus Status { get; private set; }
    public string? AssignedTechnicianId { get; private set; }
    public DateTimeOffset CreatedUtc { get; private set; }
    public DateTimeOffset? AssignedUtc { get; private set; }

    public static RepairRequest Create(
        string requesterId,
        string equipmentCode,
        string location,
        string title,
        string description,
        RepairPriority priority,
        DateTimeOffset createdAt)
    {
        var requestId = Guid.NewGuid();

        return new RepairRequest
        {
            Id = requestId,
            RequestNumber = $"RR-{createdAt:yyyyMMdd}-{requestId:N}".ToUpperInvariant(),
            RequesterId = RequireValue(requesterId, nameof(requesterId)),
            EquipmentCode = RequireValue(equipmentCode, nameof(equipmentCode)),
            Location = RequireValue(location, nameof(location)),
            Title = RequireValue(title, nameof(title)),
            Description = RequireDescription(description),
            Priority = RequirePriority(priority),
            Status = RepairStatus.New,
            CreatedUtc = createdAt
        };
    }

    public void Assign(string technicianId, string supervisorId)
    {
        if (Status != RepairStatus.New)
        {
            throw new InvalidOperationException("Only new repair requests can be assigned.");
        }

        AssignedTechnicianId = RequireValue(technicianId, nameof(technicianId));
        _ = RequireValue(supervisorId, nameof(supervisorId));
        AssignedUtc = DateTimeOffset.UtcNow;
        Status = RepairStatus.Assigned;
    }

    private static string RequireDescription(string value)
    {
        var trimmedValue = RequireValue(value, nameof(value));
        if (trimmedValue.Length > MaximumDescriptionLength)
        {
            throw new ArgumentException($"Description cannot exceed {MaximumDescriptionLength} characters.", nameof(value));
        }

        return trimmedValue;
    }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", parameterName);
        }

        return value.Trim();
    }

    private static RepairPriority RequirePriority(RepairPriority priority)
    {
        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentOutOfRangeException(nameof(priority));
        }

        return priority;
    }
}
