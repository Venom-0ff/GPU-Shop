using System.Security.Cryptography;
using CasestudyAPI.DAL;
using CasestudyAPI.DAL.DAO;
using CasestudyAPI.DAL.DomainClasses;
using CasestudyAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasestudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ControllerBase
    {
        readonly AppDbContext _db;
        public RegisterController(AppDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Index(CustomerHelper helper)
        {
            CustomerDAO dao = new(_db);
            Customer? already = await dao.GetByEmail(helper.Email);
            if (already == null)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, helper.Password!);
                helper.Password = "";
                Customer dbCustomer = new();
                dbCustomer.Firstname = helper.Firstname!;
                dbCustomer.Lastname = helper.Lastname!;
                dbCustomer.Email = helper.Email!;
                dbCustomer.Hash = hashSalt.Hash!;
                dbCustomer.Salt = hashSalt.Salt!;
                dbCustomer = await dao.Register(dbCustomer);
                if (dbCustomer.Id > 0)
                    helper.Token = "Customer registered";
                else
                    helper.Token = "Customer registration failed";
            }
            else
            {
                helper.Token = "Customer registration failed - email already in use";
            }
            return helper;
        } // end Index()

        private static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = RandomNumberGenerator.Create();

            // Fills an array of bytes with a cryptographically strong sequence of random nonzero values.
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            // a password, salt, and iteration count, then generates a binary key
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            HashSalt hashSalt = new() { Hash = hashPassword, Salt = salt };
            return hashSalt;
        } // end GenerateSaltedHash()
    }

    public class HashSalt
    {
        public string? Hash { get; set; }
        public string? Salt { get; set; }
    }
}