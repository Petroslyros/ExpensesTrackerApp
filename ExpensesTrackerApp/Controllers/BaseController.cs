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

        public readonly IApplicationService applicationService;

        public BaseController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        private ApplicationUser? appUser;
        protected ApplicationUser? AppUser
        {
            get
            {
                //  if user is extracted, return the cached version
                // (avoids parsing claims multiple times per request)
                if (appUser != null)
                    return appUser;

                //  Check if claims exist at all
                if (User?.Claims == null || !User.Claims.Any())
                    return null;

                // Look for the NameIdentifier claim (contains the user ID from JWT)
                var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (nameIdentifierClaim == null)
                    return null;

                //  Claims exist and contain user ID, so extract all user info
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
