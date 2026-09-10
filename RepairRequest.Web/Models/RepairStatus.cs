namespace RepairRequest.Web.Models;

public enum RepairStatus
{
    New = 1,
    Assigned = 2,
    InProgress = 3,
    WaitingForParts = 4,
    Completed = 5,
    Closed = 6
}
