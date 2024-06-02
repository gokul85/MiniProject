using ReturnManagementSystem.Models;

namespace ReturnManagementSystem.Repositories
{
    public class OrderRepository : BaseRepository<int, Order>
    {
        public OrderRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class OrderProductRepository : BaseRepository<int, OrderProduct>
    {
        public OrderProductRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class PolicyRepository : BaseRepository<int, Policy>
    {
        public PolicyRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class ProductRepository : BaseRepository<int, Product>
    {
        public ProductRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class ProductItemRepository : BaseRepository<int, ProductItem>
    {
        public ProductItemRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class TransactionRepository : BaseRepository<int, Transaction>
    {
        public TransactionRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class ReturnRequestRepository : BaseRepository<int, ReturnRequest>
    {
        public ReturnRequestRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class UserRepository : BaseRepository<int, User>
    {
        public UserRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }

    public class UserDetailRepository : BaseRepository<int, UserDetail>
    {
        public UserDetailRepository(ReturnManagementSystemContext context) : base(context)
        {
        }
    }
}
