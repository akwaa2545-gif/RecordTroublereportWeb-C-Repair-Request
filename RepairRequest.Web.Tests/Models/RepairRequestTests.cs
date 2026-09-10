using RepairRequest.Web.Models;
using RepairRequestEntity = global::RepairRequest.Web.Models.RepairRequest;

namespace RepairRequest.Web.Tests.Models;

public class RepairRequestTests
{
    [Fact]
    public void Create_with_valid_details_sets_new_status_and_trims_text()
    {
        var createdAt = new DateTimeOffset(2026, 9, 10, 8, 0, 0, TimeSpan.Zero);

        var request = RepairRequestEntity.Create(
            requesterId: "requester-1",
            equipmentCode: " EQ-101 ",
            location: " Line 2 ",
            title: " Conveyor belt issue ",
            description: " Belt is slipping during operation. ",
            priority: RepairPriority.High,
            createdAt);

        Assert.Equal(RepairStatus.New, request.Status);
        Assert.Equal("EQ-101", request.EquipmentCode);
        Assert.Equal("Line 2", request.Location);
        Assert.Equal("Conveyor belt issue", request.Title);
        Assert.Equal("Belt is slipping during operation.", request.Description);
        Assert.Equal(createdAt, request.CreatedUtc);
        Assert.False(string.IsNullOrWhiteSpace(request.RequestNumber));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_requires_requester_id(string requesterId)
    {
        Assert.Throws<ArgumentException>(() => CreateValidRequest(requesterId: requesterId));
    }

    [Fact]
    public void Create_rejects_description_over_2000_characters()
    {
        Assert.Throws<ArgumentException>(() => CreateValidRequest(description: new string('x', 2001)));
    }

    [Fact]
    public void New_request_can_be_assigned_by_supervisor()
    {
        var request = CreateValidRequest();

        request.Assign("technician-1", "supervisor-1");

        Assert.Equal(RepairStatus.Assigned, request.Status);
        Assert.Equal("technician-1", request.AssignedTechnicianId);
        Assert.NotNull(request.AssignedUtc);
    }

    private static RepairRequestEntity CreateValidRequest(
        string requesterId = "requester-1",
        string description = "The machine is not starting.") =>
        RepairRequestEntity.Create(
            requesterId,
            "EQ-101",
            "Line 1",
            "Machine will not start",
            description,
            RepairPriority.Normal,
            DateTimeOffset.UtcNow);
}
