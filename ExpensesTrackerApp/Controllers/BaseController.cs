using AutoMapper;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesTrackerApp.Controllers
{
    /// <summary>
    /// Base controller for API endpoints, providing common functionality across controllers.
    /// Other controllers inherit from this to get access to ApplicationService, AutoMapper,
    /// and the currently logged-in user.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        // Provides access to all services in one place (UserService, ExpenseService, etc.)
        public readonly IApplicationService applicationService;

        // Optional: AutoMapper for mapping entities to DTOs
        protected readonly IMapper mapper;

        public BaseController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        // Backing field for the logged-in user
        private ApplicationUser? appUser;

        /// <summary>
        /// Returns the currently logged-in user as an ApplicationUser object.
        /// If no user is logged in, returns null.
        /// </summary>
        protected ApplicationUser? AppUser
        {
            get
            {
                if (appUser != null)
                    return appUser; // already populated

                if (User?.Claims == null || !User.Claims.Any())
                    return null; // no claims available

                var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (nameIdentifierClaim == null)
                    return null;

                appUser = new ApplicationUser
                {
                    Id = Convert.ToInt32(nameIdentifierClaim.Value),
                    Username = User.FindFirst(ClaimTypes.Name)?.Value,
                    Email = User.FindFirst(ClaimTypes.Email)?.Value
                };

                return appUser;
            }
        }

    }
}
