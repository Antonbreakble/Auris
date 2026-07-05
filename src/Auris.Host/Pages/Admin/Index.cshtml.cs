using Auris.Host.Admin.Services;
using Auris.Host.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auris.Host.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly AdminDashboardService _dashboardService;
    public AdminDashboardViewModel? Dashboard { get; private set; }

    public IndexModel(AdminDashboardService dashboardService) {
        _dashboardService = dashboardService;
    }
    
    public void OnGet(string? tab) => Dashboard = _dashboardService.GetDashboard(tab);
    

    public IActionResult OnPostClearQueue() {
        _dashboardService.ClearQueue();

        return RedirectToPage(new
        {
            tab = AdminDashboardService.QueueTab
        });
    }
}
