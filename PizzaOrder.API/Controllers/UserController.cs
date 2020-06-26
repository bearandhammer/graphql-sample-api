using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaOrder.API.Models;
using PizzaOrder.Business.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PizzaOrder.API.Controllers
{
    /*
        NOTE: Taken verbatim from the sample code (too much to refactor at the time of taking the course)     
    */
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(
            IConfiguration config,
            SignInManager<IdentityUser> signInMan,
            UserManager<IdentityUser> userMan,
            RoleManager<IdentityRole> roleMan,
            IHttpContextAccessor httpContextAcc)
        {
            configuration = config;
            signInManager = signInMan;
            userManager = userMan;
            roleManager = roleMan;
            httpContextAccessor = httpContextAcc;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody]LoginDetails model)
        {
            // Check user exist in system or not
            IdentityUser user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound();
            }

            // Perform login operation
            Microsoft.AspNetCore.Identity.SignInResult signInResult =
                await signInManager.CheckPasswordSignInAsync(user, model.Password, true);

            if (signInResult.Succeeded)
            {
                // Obtain token
                TokenDetails token = await GetJwtSecurityTokenAsync(user);
                return Ok(token);
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreateDefaultUsers()
        {
            List<string> rolesDetails = new List<string>
            {
                Roles.Customer,
                Roles.Restaurant,
                Roles.Admin
            };

            foreach (string roleName in rolesDetails)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            Dictionary<string, IdentityUser> userDetails = new Dictionary<string, IdentityUser> {
                {
                    Roles.Customer,
                    new IdentityUser { Email = "customer@demo.com", UserName = "CustomerUser", EmailConfirmed = true }
                },
                {
                    Roles.Restaurant,
                    new IdentityUser { Email = "restaurant@demo.com", UserName = "RestaurantUser", EmailConfirmed = true }
                },
                {
                    Roles.Admin,
                    new IdentityUser { Email = "admin@demo.com", UserName = "AdminUser", EmailConfirmed = true }
                }
            };

            foreach (KeyValuePair<string, IdentityUser> details in userDetails)
            {
                IdentityUser existingUserDetails = await userManager.FindByEmailAsync(details.Value.Email);
                if (existingUserDetails == null)
                {
                    await userManager.CreateAsync(details.Value);
                    await userManager.AddPasswordAsync(details.Value, "Password");
                    await userManager.AddToRoleAsync(details.Value, details.Key);
                }
            }

            return Ok("Default User has been created");
        }

        public async Task<IActionResult> ProtectedPage()
        {
            // Obtain MailId from token
            ClaimsIdentity identity = httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            string userName = identity?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            IdentityUser user = await userManager.FindByNameAsync(userName);

            return Ok(user);
        }

        private async Task<TokenDetails> GetJwtSecurityTokenAsync(IdentityUser user)
        {
            byte[] keyInBytes = System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("JwtIssuerOptions:SecretKey").Value);
            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(keyInBytes), SecurityAlgorithms.HmacSha256);
            DateTime tokenExpireOn = DateTime.Now.AddDays(3);

            // Obtain Role of User
            IList<string> rolesOfUser = await userManager.GetRolesAsync(user);

            // Add new claims
            List<Claim> tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, rolesOfUser.FirstOrDefault()),
            };

            // Make JWT token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration.GetSection("JwtIssuerOptions:Issuer").Value,
                audience: configuration.GetSection("JwtIssuerOptions:Audience").Value,
                claims: tokenClaims,
                expires: tokenExpireOn,
                signingCredentials: credentials
            );

            // Return it
            TokenDetails TokenDetails = new TokenDetails
            {
                UserId = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireOn = tokenExpireOn,
            };

            // Set current user details for busines & common library
            IdentityUser currentUser = await userManager.FindByEmailAsync(user.Email);

            // Add new claim details
            IList<Claim> existingClaims = await userManager.GetClaimsAsync(currentUser);
            await userManager.RemoveClaimsAsync(currentUser, existingClaims);
            await userManager.AddClaimsAsync(currentUser, tokenClaims);

            return TokenDetails;
        }
    }
}
