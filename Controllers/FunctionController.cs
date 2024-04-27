using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Controllers;
using freecurrencyapi;
using freecurrencyapi.Helpers;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
	public class FunctionController : Controller
	{
		private readonly IFoodRepository _foodRepository;
		private readonly IHealthRepository _healthRepository;
		private readonly IOthersRepository _othersRepository;
		private readonly ITransferRepository _transferRepository;
		private readonly IBalanceRepository _balanceRepository;
		private readonly IUserRepository _userRepository;

		public FunctionController(IFoodRepository foodRepository,
			IHealthRepository healthRepository, IOthersRepository othersRepository,
			ITransferRepository transferRepository, IBalanceRepository balanceRepository ,
			IUserRepository userRepository)
		{
			_foodRepository = foodRepository;
			_healthRepository = healthRepository;
			_othersRepository = othersRepository;
			_transferRepository = transferRepository;
			_balanceRepository = balanceRepository;
			_userRepository = userRepository;
		}
		[HttpPost]
		public IActionResult AddFood(FormData model)
		{
			if (ModelState.IsValid)
			{
				Food newFood = new Food
				{
					Name = model.Name,
					Price = model.Price,
					Currency = model.Currency,
					UserID = (int)HttpContext.Session.GetInt32("UserId")
				};
				_foodRepository.Add(newFood);
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AddHealth(FormData model)
		{
			if (ModelState.IsValid)
			{
				Health newHealth = new Health
				{
					Name = model.Name,
					Price = model.Price,
					Currency = model.Currency,
					UserID = (int)HttpContext.Session.GetInt32("UserId")

				};
				_healthRepository.Add(newHealth);
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AddOthers(FormData model)
		{
			if (ModelState.IsValid)
			{
				Others newOthers = new Others
				{
					Name = model.Name,
					Price = model.Price,
					Currency = model.Currency,
					UserID = (int)HttpContext.Session.GetInt32("UserId")

				};
				_othersRepository.Add(newOthers);
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AddTransfer(FormData model)
		{
			if (ModelState.IsValid)
			{
				Transfers newTransfer = new Transfers
				{
					Name = model.Name,
					Price = model.Price,
					Currency = model.Currency,
					Direction = model.Who,
					UserID = (int)HttpContext.Session.GetInt32("UserId")
				};
				_transferRepository.Add(newTransfer);
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AddBalance(Balance model)
		{
			if (ModelState.IsValid)
			{
				Balance newBalance = new Balance
				{
					PLN = model.PLN,
					EURO = model.EURO,
					UserID = (int)HttpContext.Session.GetInt32("UserId")

				};
				_balanceRepository.Add(newBalance);
				return RedirectToAction("Index", "Home");
			}
			return View("/Views/Home/FormBalance.cshtml", model);
		}
		[HttpPost]
		public IActionResult AddUser(Users model)
		{
			if (_userRepository.Any(p => p.Email == model.Email))
			{
				ModelState.AddModelError("Email", "Adres Email jest zajęty");
				return View("/Views/Home/Register.cshtml", model);
			}
			if (ModelState.IsValid)
			{
				string salt = BCrypt.Net.BCrypt.GenerateSalt();
				string hashed = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);
				Users newUser = new Users
				{
					Email = model.Email,
					Password = hashed
				};
				_userRepository.Add(newUser);
				return RedirectToAction("Login", "Home");
			}
			return View("/Views/Home/Register.cshtml", model);
		}
		[HttpPost]
		public IActionResult CheckUser(UserLogin model)
		{
			if (ModelState.IsValid)
			{
				var user = _userRepository.GetByEmail(model.Email);
				if (user != null)
				{
					if (VerifyHashedPassword(model.Password, user.Password))
					{
						HttpContext.Session.SetInt32("UserId", user.Id);
						return RedirectToAction("Index", "Home");
					}
					else
					{
						ModelState.AddModelError("Password", "Nieprawidłowe hasło");
					}
				}
				else
				{
					ModelState.AddModelError("Email", "Nieprawidłowy adres Email");
				}
			}
			return View("/Views/Home/Login.cshtml", model);
		}
		public bool VerifyHashedPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Remove("UserId");
			return RedirectToAction("Login", "Home");
		}
		[HttpPost]
		public IActionResult AddTransaction(FormData model)
		{
			if (ModelState.IsValid)
			{
				switch (model.Category)
				{
					case 1:
						AddTransfer(model);
						break;
					case 2:
						AddFood(model);
						break;
					case 3:
						AddHealth(model);
						break;
					case 4:
						AddOthers(model);
						break;
				}
				return RedirectToAction("index", "Home");
			}
			return View("/Views/Home/Form.cshtml", model);
		}
		[HttpPost]
		public IActionResult AddEurExchange(string AmountEur)
		{
			if (!decimal.TryParse(AmountEur, out decimal number))
			{
				ViewBag.Error = "Nieprawidłowa wartość liczbowa";
				return View("Views/Home/ExChange.cshtml");
			}
			else
			{
				decimal numberr = Convert.ToDecimal(AmountEur);
				decimal exchangeamount = ConvertMoney(numberr, 1);
				AddTransferExchange(numberr, exchangeamount, 2, 1,"Eur -> Pln");
				return RedirectToAction("Index", "Home");
			}
		}
		[HttpPost]
		public IActionResult AddPlnExchange(string AmountPLN)
		{
			if (!decimal.TryParse(AmountPLN, out decimal numberr))
			{
				ViewBag.Error = "Nieprawidłowa wartość liczbowa";
				return View("Views/Home/ExChange.cshtml");
			}
			else
			{
				decimal number = Convert.ToDecimal(AmountPLN);
				decimal exchangeamount = ConvertMoney(number, 2);
				AddTransferExchange(number, exchangeamount,1,2,"Pln -> Eur");
				return RedirectToAction("Index", "Home");
			}
		}
		public IActionResult AddTransferExchange(decimal amount, decimal afterexchange,
			int fromCurrency, int toCurrency, string exchangeName)
		{
			int userID = (int)HttpContext.Session.GetInt32("UserId");
			Transfers newTransferMinus = new Transfers
			{
				Name = exchangeName,
				Price = amount,
				Currency = fromCurrency,
				Direction = 1,
				UserID = userID
			};
			Transfers newTransferPlus = new Transfers
			{
				Name = exchangeName,
				Price = afterexchange,
				Currency = toCurrency,
				Direction = 2,
				UserID = userID
			};
			_transferRepository.Add(newTransferMinus);
			_transferRepository.Add(newTransferPlus);
			return RedirectToAction("Index", "Home");
		}
		public decimal ConvertMoney(decimal number,int who)
		{
			string rate;
			var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
			if(who == 1)
			{
				rate = fx.Latest("EUR", "PLN");
			} else
			{
				rate = fx.Latest("PLN", "EUR");
			}
			rate = rate.Substring(15, 12);
			rate = rate.Remove(1, 1).Insert(1, ",").Remove(4, 8);
			decimal rateconverted = Convert.ToDecimal(rate);
			rateconverted = number * rateconverted;
			return rateconverted;
		}
	}
}
