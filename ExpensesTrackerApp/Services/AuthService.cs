using ExpensesTrackerApp.Core.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpensesTrackerApp.Services
{
    public class AuthService
    {
        /// <summary>
        /// Creates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="userRole">The role assigned to the user.</param>
        /// <param name="appSecurityKey">The secret key used to sign the token.</param>
        /// <returns>A signed JWT token string.</returns>
        /// <remarks>
        /// The token includes claims for username, user ID, email, and role.
        /// The issuer and audience are set to example URLs; use <c>null</c> if not needed.
        /// </remarks>
        /// 
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
                new Claim(ClaimTypes.Role, userRole.ToString()!)
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
