using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CasestudyAPI.DAL;
using CasestudyAPI.DAL.DAO;
using CasestudyAPI.DAL.DomainClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using CasestudyAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace CasestudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        readonly AppDbContext _db;
        readonly IConfiguration configuration;
        public LoginController(AppDbContext context, IConfiguration config)
        {
            _db = context;
            this.configuration = config;
        }

        [HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Index(CustomerHelper helper)
        {
            CustomerDAO dao = new(_db);
            Customer? customer = await dao.GetByEmail(helper.Email);
            if (customer != null)
            {
                if (VerifyPassword(helper.Password, customer.Hash!, customer.Salt!))
                {
                    helper.Password = "";
                    var appSettings = configuration.GetSection("AppSettings").GetValue<string>("Secret");

                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(appSettings);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, customer.Id.ToString()) }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    string returnToken = tokenHandler.WriteToken(token);
                    helper.Token = returnToken;
                }
                else
                {
                    helper.Token = "Customername or Password is invalid - login failed";
                }
            }
            else
            {
                helper.Token = "No such customer - login failed";
            }
            return helper;
        } // end Index()

        public static bool VerifyPassword(string? enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword!, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        } // end VerifyPassword()
    }
}
