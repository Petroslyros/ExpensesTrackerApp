using AutoMapper;
using ExpensesTrackerApp.Core.Enums;
using ExpensesTrackerApp.Core.Filters;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace ExpensesTrackerApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger = new LoggerFactory().AddSerilog().CreateLogger<UserService>();

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize,
            UserFiltersDTO userFiltersDTO)
        {
            List<User> users = [];
            List<Expression<Func<User, bool>>> predicates = [];


            if (!string.IsNullOrEmpty(userFiltersDTO.UserName))
            {
                predicates.Add(u => u.Username == userFiltersDTO.UserName);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.Email))
            {
                predicates.Add(u => u.Email == userFiltersDTO.Email);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.UserRole.ToString() == userFiltersDTO.UserRole);
            }

            var result = await unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);
            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = result.Data.Select(u => new UserReadOnlyDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    UserRole = u.UserRole.ToString()!
                }).ToList(),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
            };
            logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
            return dtoResult;

        }

        public async Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
        {
            try
            {
                User? user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", "User with username: " + " not found");
                }
                logger.LogInformation("User found {Username}", username);
                return new UserReadOnlyDTO
                {
                    Id = user.Id,
                    Username = username,
                    Email = user.Email,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    UserRole = user.UserRole.ToString()!
                };

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving user by username : {Username}. {Message}", username, ex.Message);
                throw;
            }
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User? user = null;
            try
            {
                user = await unitOfWork.UserRepository.GetUserAsync(credentials.Username!, credentials.Password!);

                if (user == null)
                {
                    throw new EntityNotAuthorizedException("User", "Bad Credentials");
                }
                logger.LogInformation("User with username { Username} found", credentials.Username!);
            }
            catch (EntityNotAuthorizedException ex)
            {
                logger.LogError("Authentication failed for username {Username}. {Message}", credentials.Username, ex.Message);
            }
            return user;
        }

        public string CreateUserToken(int userId, string username, string email, UserRole userRole, string appSecurityKey)
        {
            //  Create a symmetric security key from your secret string.
            // This key will be used to digitally sign the token.
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecurityKey));

            //  Define how the token will be signed — here we use HMAC-SHA256.
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //  Define the information (claims) that will be embedded inside the token.
            // used later to identify the user and their permissions.
            var claimsInfo = new List<Claim>
                {
                // The user's unique ID
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),

                // The username (used to identify the user)
                new Claim(ClaimTypes.Name, username),

                // The user's email address
                new Claim(ClaimTypes.Email, email),

                // The user's role (used for role-based authorization)
                new Claim(ClaimTypes.Role, userRole.ToString())
                };

            //  Create the JWT itself.
            //  specify the issuer, audience, claims, expiration time, and signing credentials.
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "https://localhost:5001",   // Who created and issued this token
                audience: "https://localhost:5001", // Who the token is intended for (can be your app)
                claims: claimsInfo,                 // The claims list from above
                expires: DateTime.UtcNow.AddHours(3), // Token will expire after 3 hours
                signingCredentials: signingCredentials // The digital signature info
            );

            //  Serialize (convert) the token object into a string you can return or send to the client.
            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            // Return the final token string — this is what will be given to the client.
            return userToken;
        }

    }
}
