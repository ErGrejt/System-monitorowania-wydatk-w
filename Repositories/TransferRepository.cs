using WebApplication1.Models;

namespace WebApplication1.Repositories
{
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
	public interface ITransferRepository
	{
		IEnumerable<Transfers> GetAll(int userID);
		decimal GetExpenses(int UserID, int currency, int direction);
		void Add(Transfers transfers);
	}
}
