using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Models;
using TrainTickets.Models.ViewModels;
using TrainTickets.Enums;

namespace TrainTickets.Controllers
{
    public class UserRolesController : BaseAdminController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRolesController(UserManager<ApplicationUser> userManager) 
             : base(userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!await IsAdmin())
            {
                return Forbid();
            }

            List<ApplicationUser> users = await _userManager.Users.ToListAsync();
            List<UserRolesViewModel> userRolesViewModels = new();

            foreach (ApplicationUser user in users)
            {
                UserRolesViewModel userRolesViewModel = new() 
                { 
                    UserId = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user)
                };
                userRolesViewModels.Add(userRolesViewModel);
            }

            return View(userRolesViewModels);
        }
    }
}
