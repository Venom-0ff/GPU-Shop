using CasestudyAPI.DAL;
using CasestudyAPI.DAL.DAO;
using CasestudyAPI.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasestudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandController : ControllerBase
    {
        readonly AppDbContext _db;
        public BrandController(AppDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandDAO dao = new(_db);
            List<Brand> allBrands = await dao.GetAll();
            return allBrands;
        } // end Index()
    }
}
