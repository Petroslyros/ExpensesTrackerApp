using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesTrackerApp.Controllers
{

    /// <summary>
    /// Base controller for API endpoints, providing common functionality across controllers.
    /// </summary>
    ///
    [ApiController]
    // [Produces("application/json")] default since it derives from ControllerBase
    public class BaseController : ControllerBase
    {
        public readonly IApplicationService applicationService;
        private ApplicationUser? appUser;  // this is used to cache the result of parsing the claims


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="applicationService"></param>
        /// 
        protected BaseController(IApplicationService applicationService)
        {
            applicationService = applicationService;
        }


        /// <summary>
        /// Gets the current authenticated<see cref="ApplicationUser"/> based on claims.
        /// </summary>
        /// Returns <c>null</c> if the identity is invalid or missing required claim types.
        /// 
        protected ApplicationUser? AppUser
        {
            get
            {
                if (User != null && User.Claims != null && User.Claims.Any())
                {
                    var claimsTypes = User.Claims.Select(x => x.Type);
                    if (!claimsTypes.Contains(ClaimTypes.NameIdentifier)) return null;

                    var userClaimsId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    _ = int.TryParse(userClaimsId, out int id);

                    appUser = new ApplicationUser { Id = id };

                    var userClaimsName = User.FindFirst(ClaimTypes.Name)?.Value;
                    var userClaimsEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                    appUser.Username = userClaimsName!;
                    appUser.Email = userClaimsEmail!;

                    return appUser;
                }

                return null;
            }
        }
    }
}
