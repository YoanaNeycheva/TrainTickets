using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainTickets.Enums;
using TrainTickets.Models;

namespace TrainTickets.Controllers
{
    [Authorize(Roles = "Admin")]
    public abstract class BaseAdminController : Controller
    {
        protected readonly UserManager<ApplicationUser> UserManager;

        protected BaseAdminController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        protected async Task<bool> IsAdmin()
        {
            var user = await UserManager.GetUserAsync(User);
            return user != null && user.Role == Role.Admin;
        }
    }
}
