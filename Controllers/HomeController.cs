using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Repositories;
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
		private readonly ITransferRepository _transferRepository;
		private readonly IFoodRepository _foodRepository;
		private readonly IHealthRepository _healthRepository;
		private readonly IOthersRepository _othersRepository;
		private readonly IBalanceRepository _balanceRepository;

		public HomeController(ITransferRepository transferRepository, IFoodRepository foodRepository, IHealthRepository healthRepository, IOthersRepository othersRepository, IBalanceRepository balanceRepository)
		{
			_transferRepository = transferRepository;
			_foodRepository = foodRepository;
			_healthRepository = healthRepository;
			_othersRepository = othersRepository;
			_balanceRepository = balanceRepository;
		}

		public IActionResult Index()
		{
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				int IDUser = (int)HttpContext.Session.GetInt32("UserId");
				ViewData["ID"] = IDUser;
				ViewData["TransferData"] = _transferRepository.GetAll(IDUser);
				ViewData["FoodData"] = _foodRepository.GetAll(IDUser);
				ViewData["HealthData"] = _healthRepository.GetAll(IDUser);
				ViewData["OthersData"] = _othersRepository.GetAll(IDUser);
				ViewData["BalanceData"] = _balanceRepository.GetAll(IDUser);
				ViewData["ExpensesOthers"] = _othersRepository.GetExpenses(IDUser,1);
				ViewData["ExpensesFood"] = _foodRepository.GetExpenses(IDUser,1);
				ViewData["ExpensesHealth"] = _healthRepository.GetExpenses(IDUser,1);
				ViewData["ExpensesOthersEur"] = _othersRepository.GetExpenses(IDUser,2);
				ViewData["ExpensesFoodEur"] = _foodRepository.GetExpenses(IDUser,2);
				ViewData["ExpensesHealthEur"] = _healthRepository.GetExpenses(IDUser,2);
				//Currency, Direction
				ViewData["ExpensesOutPln"] = _transferRepository.GetExpenses(IDUser,1,1);
				ViewData["ExpensesInPln"] = _transferRepository.GetExpenses(IDUser,1,2);
				ViewData["ExpensesOutEur"] = _transferRepository.GetExpenses(IDUser,2,1);
				ViewData["ExpensesInEur"] = _transferRepository.GetExpenses(IDUser,2,2);

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
				var BalanceData = _balanceRepository.GetAll(IDUser);
				var RowCount = BalanceData.Count();
				if(RowCount == 0)
				{
					return RedirectToAction("FormBalance", "Home");
				} else
				{
					return View();
				}
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
		public IActionResult FormBalance()
		{
			return View();
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
