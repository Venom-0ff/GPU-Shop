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
    public class ProductController : ControllerBase
    {
        readonly AppDbContext _db;

        public ProductController(AppDbContext ctx)
        {
            _db = ctx;
        }

        [HttpGet]
        [Route("{brid}")]
        public async Task<ActionResult<List<Product>>> Index(int brid)
        {
            ProductDAO dao = new(_db);
            List<Product> itemsForBrand = await dao.GetAllByBrand(brid);
            return itemsForBrand;
        } // end Index()
    }
}
