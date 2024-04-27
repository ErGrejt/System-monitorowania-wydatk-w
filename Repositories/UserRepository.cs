using System.Linq.Expressions;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
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
	public interface IUserRepository
	{
		Users GetByEmail(string email);
		void Add(Users users);
		bool Any(Expression<Func<Users, bool>> predicate);
	}
}
