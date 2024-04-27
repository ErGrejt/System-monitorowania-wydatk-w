using WebApplication1.Models;

namespace WebApplication1.Repositories
{
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
	public interface IHealthRepository
	{
		IEnumerable<Health> GetAll(int userId);
		decimal GetExpenses(int userID, int currency);
		void Add(Health health);
	}
}
