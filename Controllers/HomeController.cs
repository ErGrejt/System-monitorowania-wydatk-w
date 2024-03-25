using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using WebApplication1.Controllers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Http;
using System.Text.Json;
using freecurrencyapi;
using freecurrencyapi.Helpers;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;

		public HomeController(AppDbContext context, ILogger<HomeController> logger)
		{
			_context = context;
			_logger = logger;
		}

		public IActionResult Index()
		{
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				int IDUser = (int)HttpContext.Session.GetInt32("UserId");
				ViewData["ID"] = IDUser;
				string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
				MySqlConnection connection = new MySqlConnection(connectionString);
				connection.Open();
				ViewData["TransferData"] = _context.Transfers.Where(p => p.UserID == IDUser).ToList();
				ViewData["FoodData"] = _context.Food.Where(p => p.UserID == IDUser).ToList();
				ViewData["HealthData"] = _context.Health.Where(p => p.UserID == IDUser).ToList();
				ViewData["OthersData"] = _context.Others.Where(p => p.UserID == IDUser).ToList();
				ViewData["BalanceData"] = _context.Balance.Where(p => p.UserID == IDUser).ToList();
				ViewData["ExpensesOthers"] = _context.Others
					.Where(z => z.Currency == 1 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesFood"] = _context.Food
					.Where(z => z.Currency == 1 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesHealth"] = _context.Health
					.Where(z => z.Currency == 1 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesOthersEur"] = _context.Others
					.Where(z => z.Currency == 2 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesFoodEur"] = _context.Food
					.Where(z => z.Currency == 2 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesHealthEur"] = _context.Health
					.Where(z => z.Currency == 2 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesOutPln"] = _context.Transfers
					.Where(z => z.Currency == 1 && z.Direction == 1 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesInPln"] = _context.Transfers
					.Where(z => z.Currency == 1 && z.Direction == 2 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesOutEur"] = _context.Transfers
					.Where(z => z.Currency == 2 && z.Direction == 1 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();
				ViewData["ExpensesInEur"] = _context.Transfers
					.Where(z => z.Currency == 2 && z.Direction == 2 && z.UserID == IDUser)
					.Select(z => z.Price)
					.Sum();

				connection.Close();
				var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
				string Currencies = fx.Latest("EUR", "PLN");
				Currencies = Currencies.Substring(15, 12);
				Currencies = Currencies.Remove(1, 1).Insert(1, ",").Remove(4, 8);
				ViewData["EuroRate"] = Currencies;

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		public IActionResult Form()
		{
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				var IDUser = (int)HttpContext.Session.GetInt32("UserId");
				var BalanceData = _context.Balance
							.Where(s => s.UserID == IDUser)
							.ToList();
				ViewData["BalanceData"] = BalanceData;
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}
		public IActionResult ExChange()
		{
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				//Klucz freecurrency
				var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
				
				string EurToPlnRate = fx.Latest("EUR", "PLN");
                EurToPlnRate = EurToPlnRate.Substring(15, 12);
                EurToPlnRate = EurToPlnRate.Remove(1, 1).Insert(1, ",").Remove(4, 8);
				ViewData["EurToPlnRate"] = EurToPlnRate;
				
				string PlnToEurRate = fx.Latest("PLN", "EUR");
                PlnToEurRate = PlnToEurRate.Substring(15, 12);
                PlnToEurRate = PlnToEurRate.Remove(1, 1).Insert(1, ",").Remove(4, 8);
				ViewData["PlnToEurRate"] = PlnToEurRate;

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}
		public IActionResult Login()
		{
			var error = TempData["error"] as string;
			ViewData["Error"] = error;
			return View();
		}
		public IActionResult Register()
		{
			var errorregister = TempData["Email"] as string;
			ViewData["ErrorReg"] = errorregister;
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

	}
}
