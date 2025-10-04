using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.Data;
using B2B_API.Infrastructure.Repositories;

namespace B2B_API.Infrastructure.Persistence
{
    /// <summary>
    /// Реализация Unit of Work паттерна
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();

            // Инициализируем репозитории
            Users = new EfRepository<User>(_context);
            Products = new EfRepository<Product>(_context);
            Categories = new EfRepository<Category>(_context);
            Orders = new EfRepository<Order>(_context);
            OrderItems = new EfRepository<OrderItem>(_context);
            PriceLists = new EfRepository<PriceList>(_context);
            PriceListProducts = new EfRepository<PriceListProduct>(_context);
        }

        public IRepository<User> Users { get; }
        public IRepository<Product> Products { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderItem> OrderItems { get; }
        public IRepository<PriceList> PriceLists { get; }
        public IRepository<PriceListProduct> PriceListProducts { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
