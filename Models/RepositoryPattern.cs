using System.Linq.Expressions;

namespace WebApplication1.Models
{
	public class RepositoryPattern
	{
	}
	//Interfaces
	public interface ITransferRepository
	{
		IEnumerable<Transfers> GetAll(int userID);
		decimal GetExpenses(int UserID, int currency, int direction);
		void Add(Transfers transfers);
	}
	public interface IFoodRepository
	{
		IEnumerable<Food> GetAll(int userId);
		decimal GetExpenses(int userID, int currency);
		void Add(Food food);
	}
    public interface IHealthRepository
    {
        IEnumerable<Health> GetAll(int userId);
		decimal GetExpenses(int userID, int currency);
		void Add(Health health);
	}
	public interface IOthersRepository
	{
		IEnumerable<Others> GetAll(int userId);
		decimal GetExpenses(int userID, int currency);
		void Add(Others others);
	}
	public interface IBalanceRepository
	{
		IEnumerable<Balance> GetAll(int userId);
		void Add(Balance balance);
	}
	public interface IUserRepository
	{
		Users GetByEmail(string email);
		void Add(Users users);
		bool Any(Expression<Func<Users, bool>> predicate);
	}
	//Classes
	public class TransferRepository : ITransferRepository
	{
		private readonly AppDbContext _context;
		public TransferRepository(AppDbContext context)
		{
			_context = context;
		}
		public IEnumerable<Transfers> GetAll(int userId)
		{
			return _context.Transfers.Where(p => p.UserID == userId).ToList();
		}
		public decimal GetExpenses(int UserID, int currency, int direction)
		{
			return _context.Transfers.Where(p => p.Currency == currency && p.Direction == direction && p.UserID == UserID)
				   .Select(p => p.Price).Sum();
		}
		public void Add(Transfers transfers)
		{
			_context.Transfers.Add(transfers);
			_context.SaveChanges();
		}
	}
	public class FoodRepository : IFoodRepository
	{
		private readonly AppDbContext _context;
		public FoodRepository(AppDbContext context)
		{
			_context = context;
		}
		public IEnumerable<Food> GetAll(int userId)
		{
			return _context.Food.Where(p => p.UserID == userId).ToList();
		}
		public decimal GetExpenses(int userId, int currency)
		{
			return _context.Food.Where(p => p.Currency == currency && p.UserID == userId).Select(p => p.Price).Sum();
		}
		public void Add(Food food) {
			_context.Food.Add(food);
			_context.SaveChanges();
		}
		
	}
	public class HealthRepository : IHealthRepository
	{
		private readonly AppDbContext _context;
		public HealthRepository(AppDbContext context)
		{
			_context = context;
		}
		public IEnumerable<Health> GetAll(int userId)
		{
			return _context.Health.Where(p => p.UserID == userId).ToList();
		}
		public decimal GetExpenses(int userId, int currency)
		{
			return _context.Health.Where(p => p.Currency == currency && p.UserID == userId).Select(p => p.Price).Sum();
		}
		public void Add(Health health)
		{
			_context.Health.Add(health);
			_context.SaveChanges();
		}
	}
	public class OthersRepository : IOthersRepository
	{
		private readonly AppDbContext _context;
		public OthersRepository(AppDbContext context)
		{
			_context = context;
		}
		public IEnumerable<Others> GetAll(int userId)
		{
			return _context.Others.Where(p => p.UserID == userId).ToList();
		}
		public decimal GetExpenses(int userId, int currency)
		{
			return _context.Others.Where(p => p.Currency == currency && p.UserID == userId).Select(p => p.Price).Sum();
		}
		public void Add(Others other)
		{
			_context.Others.Add(other);
			_context.SaveChanges();
		}
	}
	public class BalanceRepository : IBalanceRepository
	{
		private readonly AppDbContext _context;
		public BalanceRepository(AppDbContext context)
		{
			_context = context;
		}
		public IEnumerable<Balance> GetAll(int userId)
		{
			return _context.Balance.Where(p => p.UserID == userId).ToList();
		}
		public void Add(Balance balance)
		{
			_context.Balance.Add(balance);
			_context.SaveChanges();
		}
	}
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _context;
		public UserRepository(AppDbContext context)
		{
			_context = context;
		}
		public Users GetByEmail(string email)
		{
			return _context.Users.FirstOrDefault(p => p.Email == email);
		}
		public void Add(Users users)
		{
			_context.Users.Add(users);
			_context.SaveChanges();
		}
		public bool Any(Expression<Func<Users, bool>> predicate)
		{
			return _context.Users.Any(predicate);
		}
	}
}
