using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Endpoint.Models.Dtos.AdminDto;
using Taktamir.framework.Common.JobsUtill;
using Taktamir.framework.Common.OrderUtiil;

namespace Taktamir.Endpoint.Controllers.Admin
{
    [Authorize(Roles = UserRoleApp.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;

        public FinancialController(UserManager<User> userManager,IUserRepository userRepository, IWalletRepository walletRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _walletRepository = walletRepository;
        }
        [HttpGet("OrdersOutstanding")]
        public async Task<IActionResult> jobsCompelt(int page = 1, int pageSize = 10)
        {
            var result = await _walletRepository.GetAllOrdersOutstandingbyAdmin(page, pageSize);
            var resultUsers = new List<UserOrderDto>();
            foreach (var user in result.Item1)
            {
                
                if (user.Wallet.Orders.Count() > 0 )
                {
                    var Userdata = new UserOrderDto();
                    Userdata.Userid = user.Id;
                    Userdata.Fullname = $"{user.Firstname} {user.LastName}";
                    Userdata.PhoneNumber = user.PhoneNumber;
                    foreach (var order in user.Wallet.Orders)
                    {
                        if (order.PaymentStatus == PaymentStatus.unpaid )
                        {
                            var orderdto = new OrdersUserDto();
                            orderdto.PaymentStatus = OrderUtills.StatusPayment((int)order.PaymentStatus);
                            orderdto.OrderId = order.Id;
                            orderdto.Total = order.Total;
                            orderdto.Spent = order.spent;
                            if (order.OrderJobs.Select(p=>p.Job.StatusJob).FirstOrDefault()==StatusJob.Completed)
                            {
                                foreach (var job in order.OrderJobs)
                                {
                                    if (job.Job.StatusJob.Equals(StatusJob.Completed))
                                    {
                                        orderdto.TitletJob = job.Job.Name_Device;
                                        orderdto.jobId = job.Job.Id;
                                        orderdto.NameCustomer = job.Job.Customer.FullNameCustomer;
                                    }
                                }
                                Userdata.Orders.Add(orderdto);
                            }
                           
                        }
                       
                    }
                    resultUsers.Add(Userdata);
                }
            }
            return new JsonResult(new { Paginationdata = result.Item2, Data = resultUsers });
        }

        [HttpPost("{OrderId}/{UserId}")]
        public async Task<IActionResult> Payoff(int OrderId, int UserId ,CancellationToken cancellationToken,float Percent= 40)
        {
            var User = await _userManager.Users.Include(p=>p.Room).Include(p => p.Wallet).ThenInclude(p=>p.Orders).FirstOrDefaultAsync(p => p.Id == UserId);
            var order = User.Wallet.Orders.FirstOrDefault(p => p.Id == OrderId);
            if (order == null) return NotFound();
        
           var Amount= CalculatePercent(order.Total, Percent);
            User.Wallet.Diposit(Amount);
            order.PaymentStatus = PaymentStatus.paid;

            await _walletRepository.UpdateAsync(User.Wallet, cancellationToken);
            return Ok("Successfull");
        }
        
        [HttpGet("Cleared")]
        public async Task<IActionResult> Cleared(int page = 1, int pageSize = 10)
        {
            var result = await _walletRepository.GetAllOrdersClearedbyAdmin(page, pageSize);
            var resultUsers = new List<UserOrderDto>();
            foreach (var user in result.Item1)
            {

                if (user.Wallet.Orders.Count() > 0)
                {
                    var Userdata = new UserOrderDto();
                    Userdata.Userid = user.Id;
                    Userdata.Fullname = $"{user.Firstname} {user.LastName}";
                    Userdata.PhoneNumber = user.PhoneNumber;
                    foreach (var order in user.Wallet.Orders)
                    {
                        if (order.PaymentStatus == PaymentStatus.paid)
                        {
                            var orderdto = new OrdersUserDto();
                            orderdto.PaymentStatus = OrderUtills.StatusPayment((int)order.PaymentStatus);
                            orderdto.OrderId = order.Id;
                            orderdto.Total = order.Total;
                            orderdto.Spent = order.spent;
                            if (order.OrderJobs.Select(p => p.Job.StatusJob).FirstOrDefault() == StatusJob.Completed)
                            {
                                foreach (var job in order.OrderJobs)
                                {
                                    if (job.Job.StatusJob.Equals(StatusJob.Completed))
                                    {
                                        orderdto.TitletJob = job.Job.Name_Device;
                                        orderdto.jobId = job.Job.Id;
                                        orderdto.NameCustomer = job.Job.Customer.FullNameCustomer;
                                    }
                                }
                                Userdata.Orders.Add(orderdto);
                            }

                        }

                    }
                    resultUsers.Add(Userdata);
                }
            }
            return new JsonResult(new { Paginationdata = result.Item2, Data = resultUsers });

        }

        [HttpGet("OrderDetaills/{OrderId}")]
        public async Task<IActionResult> OrderDetaills(int OrderId)
        {
            var order=await _walletRepository.GetOrderDetails(OrderId);
            if (order is null) return NotFound();
            var result = new OrderDetaillDto();
            result.Total = order.Total;
            result.Proplem = order.OrderJobs.Select(p => p.Job.Problems).FirstOrDefault();
            result.NameDevice = order.OrderJobs.Select(p => p.Job.Name_Device).FirstOrDefault();
            result.StatrusPayment = OrderUtills.StatusPayment((int)order.PaymentStatus);
            result.fullNameCustomer= order.OrderJobs.Select(p=>p.Job.Customer.FullNameCustomer).FirstOrDefault();
            foreach (var job in order.OrderJobs)
            {
                
                foreach (var item in job.Job.Supplies)
                {
                  var  suply= new SuppliesOrderDto()
                    {
                        Name = item.Name,
                        Price = item.Price
                  
                    };
                    result.Supplies.Add(suply);
                }
            }
          // result.Supplies= order.OrderJobs.SelectMany(p=>p.Job.Supplies).ToList();
            return Ok(result);
        }

        private double CalculatePercent(double Total,float Percent)
        {
            float percent1 = Percent / 100;
            double Amount = Total * percent1;
            double finalAmount = Total - Math.Round(Amount); ;
            return finalAmount;
        }
    }
}
