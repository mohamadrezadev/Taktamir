using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.infra.Data.sql._06.Wallets;
using Xunit;

namespace Tests.Reposities
{
    public class WalletRepositoryTest
    {
        private readonly IWalletRepository _walletRepository;
        private readonly AppDbContext _appDbContext;


     
        [Fact]
        public async Task GetAllOrders_ReturnsListOfOrdersForGivenUserId()
        {
            // Arrange
            var wallet = new Wallet
            {
                Id = 1,
                UserId = 1,
                Orders = new List<Order>
                {
                    new Order {Id = 1, Total = 10},
                    new Order {Id = 2, Total = 20},
                    new Order {Id = 3, Total = 30}
                }
            };

           // var wallet = new Wallet { Id = 1 };
            var order = new Order { Id = 1 };

            var mockDbSet = new Mock<DbSet<Wallet>>();
            mockDbSet.Setup(x => x.FindAsync(wallet.Id))
                     .ReturnsAsync(wallet);

           
            var mockDbContext = new Mock<AppDbContext>();
            mockDbContext.Setup(x => x.Wallets)
                         .Returns(mockDbSet.Object);
            var service = new WalletsRepository(mockDbContext.Object);

            // Act
            var result = await service.GetAllOrders(wallet.UserId);
            Assert.IsTrue(result.Count>0);

        }
  
        [Test]
        public async Task AddNewOrder_ShouldAddOrderToWallet_WithMock()
        {
            // Arrange
            var wallet = new Wallet { Id = 1 };
            var order = new Order { Id = 1 };

            var mockDbSet = new Mock<DbSet<Wallet>>();
            mockDbSet.Setup(x => x.FindAsync(wallet.Id))
                     .ReturnsAsync(wallet);

            var mockDbContext = new Mock<AppDbContext>();
            mockDbContext.Setup(x => x.Wallets)
                         .Returns(mockDbSet.Object);

            var service = new WalletsRepository(mockDbContext.Object);

            // Act
            var result = await service.AddNewOrder(wallet.Id, order);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, wallet.Orders.Count);
            Assert.AreEqual(order, wallet.Orders.First());
        }
        
        //[Test]
        //public async Task CreateWallet_WithValidWallet_ReturnsTrue()
        //{
        //    // Arrange
        //    var dbContextMock = new Mock<AppDbContext>();
        //    var user = new User { Id = 1, LastName = "kiani" };
        //    var wallet = new Wallet { Id = 1, UserId = user.Id, };
        //    wallet.User = user;
        //    wallet.Diposit(10);
        //    dbContextMock.Setup(m => m.Users.Include(p=>p.Wallet).FirstOrDefault(p=>p.Id==user.Id)).Returns(user);




        //    var service = new WalletsRepository(dbContextMock.Object);

        //    // Act
        //    var result = await service.CreateWallet(wallet);

        //    // Assert
        //    //dbContextMock.Verify(m => m.Wallets.AddAsync(wallet), Times.Once);
        //    //dbContextMock.Verify(m => m.SaveChangesAsync(), Times.Once);
        //    Assert.IsTrue(result);
        //}
        //[Test]
        //public async Task GetAllOrders_ReturnsListOfOrders()
        //{
        //    // Arrange
        //    var dbContextMock = new Mock<AppDbContext>();
        //    var user = new User { Id = 1 };
        //    var wallet = new Wallet { Id = 1, UserId = user.Id, Orders = new List<Order> { new Order(), new Order() } };

        //    dbContextMock.Setup(m => m.Wallets.FindAsync(wallet.Id))
        //            .ReturnsAsync(wallet);

        //    var service = new WalletsRepository(dbContextMock.Object);

        //    // Act
        //    var result = await service.GetAllOrders(user.Id);

        //    // Assert
        //    Assert.AreEqual(2, result.Count);
        //    Assert.AreEqual(wallet.Orders.First(), result.First());
        //    Assert.AreEqual(wallet.Orders.Last(), result.Last());
        //}

    }
    
    public class WalletRepositoryTest1
    {

    }
    public class InMemoryDbContextFactory
    {
        public AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseSqlite("Data Source=LocalDatabase.db")
                            // and also tried using SqlLite approach. But same issue reproduced.
                            .Options;
            var dbContext = new AppDbContext(options);

            return dbContext;
        }
    }
}