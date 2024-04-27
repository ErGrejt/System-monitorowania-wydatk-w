using WebApplication1.Models;

namespace WebApplication1.Repositories
{
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
	public interface IBalanceRepository
	{
		IEnumerable<Balance> GetAll(int userId);
		void Add(Balance balance);
	}
}
