using CasestudyAPI.DAL.DomainClasses;
using CasestudyAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CasestudyAPI.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<int> AddOrder(int custid, OrderSelectionHelper[] selections)
        {
            int orderid = -1;

            using (var _trans = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    Order order = new();
                    order.CustomerId = custid;
                    order.OrderDate = DateTime.Now;
                    order.OrderAmount = 0.0M;
                    
                    foreach (OrderSelectionHelper selection in selections)
                    {
                        order.OrderAmount += selection.Product!.MSRP * selection.Qty;
                    }
                    await _db.Orders!.AddAsync(order);
                    await _db.SaveChangesAsync();

                    foreach (OrderSelectionHelper selection in selections)
                    {
                        OrderLineItem orderLine = new();
                        Product prod = selection.Product!;

                        if (selection.Qty <= prod.QtyOnHand)
                        {
                            prod.QtyOnHand -= selection.Qty;
                            orderLine.QtySold = selection.Qty;
                            orderLine.QtyOrdered = selection.Qty;
                            orderLine.QtyBackOrdered = 0;
                            orderLine.ProductId = selection.Product!.Id;
                            orderLine.OrderId = order.Id;
                            orderLine.SellingPrice = prod.MSRP;
                        }
                        else
                        {
                            prod.QtyOnBackOrder += selection.Qty - prod.QtyOnHand;
                            orderLine.QtySold = prod.QtyOnHand;
                            orderLine.QtyOrdered = selection.Qty;
                            orderLine.QtyBackOrdered = selection.Qty - prod.QtyOnHand;
                            orderLine.ProductId = selection.Product!.Id;
                            prod.QtyOnHand = 0;
                            orderLine.OrderId = order.Id;
                            orderLine.SellingPrice = prod.MSRP;
                        }
                        await _db.OrderLineItems!.AddAsync(orderLine);
                        _db.Products!.Update(prod);
                        await _db.SaveChangesAsync();
                    }

                    await _trans.CommitAsync();
                    orderid = order.Id;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await _trans.RollbackAsync();
                }
            } // end transaction

            return orderid;
        } // end AddOrder()

        public async Task<List<Order>> GetAll(int id)
        {
            return await _db.Orders!.Where(order => order.CustomerId == id).ToListAsync<Order>();
        } // end GetAll()

        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int tid, string email)
        {
            Customer? customer = _db.Customers!.FirstOrDefault(user => user.Email == email);
            List<OrderDetailsHelper> allDetails = new();
            // LINQ way of doing INNER JOINS
            var results = from o in _db.Orders
                          join oi in _db.OrderLineItems! on o.Id equals oi.OrderId
                          join p in _db.Products! on oi.ProductId equals p.Id
                          where (o.CustomerId == customer!.Id && o.Id == tid)
                          select new OrderDetailsHelper
                          {
                              OrderId = o.Id,
                              QtySold = oi.QtySold,
                              QtyOrdered = oi.QtyOrdered,
                              QtyBackOrdered = oi.QtyBackOrdered,
                              Price = oi.SellingPrice,
                              ProductName = p.ProductName,
                              ProductId = oi.ProductId,
                              OrderDate = o.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                              
                          };
            allDetails = await results.ToListAsync();
            return allDetails;
        } // end GetOrderDetails
    }
}
