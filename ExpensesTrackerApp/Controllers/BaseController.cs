using AutoMapper;
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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        public readonly IApplicationService applicationService;
        protected readonly IMapper mapper;

        public BaseController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        private ApplicationUser? appUser;
        protected ApplicationUser? AppUser
        {
            get
            {
                if (appUser != null && User.Claims != null && User.Claims.Any())
                {
                    var claimsType = User.Claims.Select(c => c.Type).ToList();
                    if (!claimsType.Contains(ClaimTypes.NameIdentifier))
                    {
                        return null;
                    }
                    var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    appUser = new ApplicationUser
                    {
                        Id = userId,
                        Username = User.FindFirst(ClaimTypes.Name)?.Value,
                        Email = User.FindFirst(ClaimTypes.Email)?.Value
                    };
                    return appUser;
                }
                return null;
            }
        }
    }
}
