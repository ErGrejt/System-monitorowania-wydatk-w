using WebApplication1.Models;

namespace WebApplication1.Repositories
{
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
	public interface IOthersRepository
	{
		IEnumerable<Others> GetAll(int userId);
		decimal GetExpenses(int userID, int currency);
		void Add(Others others);
	}
}
