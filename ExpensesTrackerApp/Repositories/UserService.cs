using AutoMapper;
using ExpensesTrackerApp.Core.Enums;
using ExpensesTrackerApp.Core.Filters;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace ExpensesTrackerApp.Repositories
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
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, userRole.ToString())
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: claimsInfo,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials
            );

            // Serialize the token to a string
            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return userToken;
        }
    }
}
