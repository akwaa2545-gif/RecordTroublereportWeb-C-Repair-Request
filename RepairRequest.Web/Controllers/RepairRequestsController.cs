using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairRequest.Web.Data;
using RepairRequest.Web.Models;
using RepairRequest.Web.ViewModels.RepairRequests;
using RepairRequestEntity = global::RepairRequest.Web.Models.RepairRequest;

namespace RepairRequest.Web.Controllers;

[Authorize]
public class RepairRequestsController(ApplicationDbContext databaseContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var requesterId = GetRequesterId();
        var requests = await databaseContext.RepairRequests
            .AsNoTracking()
            .Where(request => request.RequesterId == requesterId)
            .OrderByDescending(request => request.CreatedUtc)
            .ToListAsync();

        return View(requests);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateRepairRequestViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRepairRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var repairRequest = RepairRequestEntity.Create(
            GetRequesterId(),
            model.EquipmentCode,
            model.Location,
            model.Title,
            model.Description,
            RepairPriority.Normal,
            DateTimeOffset.UtcNow);

        databaseContext.RepairRequests.Add(repairRequest);
        await databaseContext.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Repair request {repairRequest.RequestNumber} was submitted.";

        return RedirectToAction(nameof(Index));
    }

    private string GetRequesterId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The authenticated user identifier is missing.");
}
