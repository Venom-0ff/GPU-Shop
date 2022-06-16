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
    public class OrderController : ControllerBase
    {
        readonly AppDbContext _ctx;
        public OrderController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string result;

            try
            {
                CustomerDAO custDao = new(_ctx);
                Customer? cust = await custDao.GetByEmail(helper.Email);
                OrderDAO orderDao = new(_ctx);
                int orderId = await orderDao.AddOrder(cust!.Id, helper.Selections!);

                result = orderId > 0 ? "Order " + orderId + " saved!" : "Order not saved :(";
            }
            catch (Exception e)
            {
                result = "Order not saved " + e.Message;
            }
            
            return result;
        } // end Index()

        [Route("{email}")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> orders; ;
            CustomerDAO dao = new(_ctx);
            Customer? orderOwner = await dao.GetByEmail(email);
            OrderDAO oDao = new(_ctx);
            orders = await oDao.GetAll(orderOwner!.Id);
            return orders;
        } // end List()

        [Route("{orderid}/{email}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            OrderDAO dao = new(_ctx);
            return await dao.GetOrderDetails(orderid, email);
        } // end GetOrderDetails()
    }
}
