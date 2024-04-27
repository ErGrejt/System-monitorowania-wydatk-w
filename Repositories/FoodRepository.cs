using WebApplication1.Models;

namespace WebApplication1.Repositories
{
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
		public void Add(Food food)
		{
			_context.Food.Add(food);
			_context.SaveChanges();
		}
	}
	public interface IFoodRepository
	{
		IEnumerable<Food> GetAll(int userId);
		decimal GetExpenses(int userID, int currency);
		void Add(Food food);
	}
}
