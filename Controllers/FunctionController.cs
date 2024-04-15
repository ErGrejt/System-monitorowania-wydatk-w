using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
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
		private readonly AppDbContext _context;


		public FunctionController(AppDbContext context)
		{
			_context = context;

		}

		[HttpPost]
		//Dodanie do tabeli Jedzenie
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
				_context.Food.Add(newFood);
				_context.SaveChanges();
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		//Dodanie do tabeli Zdrowie
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
				_context.Health.Add(newHealth);
				_context.SaveChanges();
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		//Dodanie do tabeli Rozne
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

				_context.Others.Add(newOthers);
				_context.SaveChanges();
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		//Dodanie do tabeli Przelewy
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
				_context.Transfers.Add(newTransfer);
				_context.SaveChanges();
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}

		//Dodanie do tabeli Saldo
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
				_context.Balance.Add(newBalance);
				_context.SaveChanges();
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		//Funkcje dodawania
		public IActionResult AddTransferEurtoPln(decimal euramount, decimal plnafterexchange)
		{
			Transfers newTransferExchangeEurMinus = new Transfers
			{
				Name = "Wymiana Eur->Pln",
				Price = euramount,
				Currency = 2,
				Direction = 1,
				UserID = (int)HttpContext.Session.GetInt32("UserId")
			};
			Transfers newTransferExchangeOPlnPlus = new Transfers
			{
				Name = "Wymiana Eur->Pln",
				Price = plnafterexchange,
				Currency = 1,
				Direction = 2,
				UserID = (int)HttpContext.Session.GetInt32("UserId")
			};
			_context.Transfers.Add(newTransferExchangeEurMinus);
			_context.SaveChanges();
			_context.Transfers.Add(newTransferExchangeOPlnPlus);
			_context.SaveChanges();


			return RedirectToAction("Index", "Home");
		}
		public IActionResult AddTransferPlntoEur(decimal plnamount, decimal eurafterexchange)
		{
			Transfers newTransferPlnExchangeMinus = new Transfers
			{
				Name = "Wymiana Pln->Eur",
				Price = plnamount,
				Currency = 1,
				Direction = 1,
				UserID = (int)HttpContext.Session.GetInt32("UserId")
			};
			Transfers newTransferEurExchangePlus = new Transfers
			{
				Name = "Wymiana Pln->Eur",
				Price = eurafterexchange,
				Currency = 2,
				Direction = 2,
				UserID = (int)HttpContext.Session.GetInt32("UserId")
			};
			_context.Transfers.Add(newTransferPlnExchangeMinus);
			_context.SaveChanges();
			_context.Transfers.Add(newTransferEurExchangePlus);
			_context.SaveChanges();


			return RedirectToAction("Index", "Home");
		}
		[HttpPost]
		public IActionResult AddUser(Users model)
		{
			if (ModelState.IsValid)
			{
				if (IsValidEmail(model.Email))
				{
					if (!_context.Users.Any(p => p.Email == model.Email))
					{
						if (model.Password.Length >= 8)
						{
							string salt = BCrypt.Net.BCrypt.GenerateSalt();
							string password = model.Password;
							string hashed = BCrypt.Net.BCrypt.HashPassword(password, salt);
							Users newUser = new Users
							{
								Email = model.Email,
								Password = hashed
							};
							_context.Users.Add(newUser);
							_context.SaveChanges();
							return RedirectToAction("Login", "Home");
						}
						else
						{
							TempData["Email"] = "Hasło musi składac się z conajmniej 8 znaków";
						}
					} else
					{
						TempData["Email"] = "Adres Email jest zajęty";
					}
				} else
				{
					TempData["Email"] = "Nieprawidłowy Email";
				}
			}

			return RedirectToAction("Register", "Home");
		}
		public bool IsValidEmail(string email)
		{
			string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
			return Regex.IsMatch(email, pattern);
		}
		[HttpPost]
		public IActionResult CheckUser(UserLogin model)
		{
			if (ModelState.IsValid)
			{

				var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
				if (user != null)
				{
					if (VerifyHashedPassword(model.Password, user.Password))
					{
						HttpContext.Session.SetInt32("UserId", user.Id);
						return RedirectToAction("Index", "Home");
					}
					else
					{
						TempData["error"] = "Nieprawidłowe hasło";
					}

				}
				else
				{
					TempData["error"] = "Nieprawidłowy adres e-mail";
				}
			}

			return RedirectToAction("Login", "Home");
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
			return RedirectToAction("Form", "Home");
		}

		[HttpPost]
		public IActionResult AddEurExchange(string AmountEur)
		{
			if (!decimal.TryParse(AmountEur, out decimal number))
			{
				return RedirectToAction("ExChange", "Home");
			}
			else
			{
				decimal numberr = Convert.ToDecimal(AmountEur);
				//Klucz freecurrency
				var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
				string EurtoPlnRate = fx.Latest("EUR", "PLN");
				EurtoPlnRate = EurtoPlnRate.Substring(15, 12);
				EurtoPlnRate = EurtoPlnRate.Remove(1, 1).Insert(1, ",").Remove(4, 8);
				decimal Eurorate = Convert.ToDecimal(EurtoPlnRate);
				decimal amountPLN = numberr * Eurorate;
				AddTransferEurtoPln(numberr, amountPLN);
				return RedirectToAction("Index", "Home");
			}
		}
		[HttpPost]
		public IActionResult AddPlnExchange(string AmountPLN)
		{
			if (!decimal.TryParse(AmountPLN, out decimal numberr))
			{
				return RedirectToAction("ExChange", "Home");
			}
			else
			{
				decimal number = Convert.ToDecimal(AmountPLN);
				//Klucz freecurrency
				var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
				string PlntoEurRate = fx.Latest("PLN", "EUR");
				PlntoEurRate = PlntoEurRate.Substring(15, 12);
				PlntoEurRate = PlntoEurRate.Remove(1, 1).Insert(1, ",").Remove(4, 8);
				decimal Plnrate = Convert.ToDecimal(PlntoEurRate);
				decimal amountEUR = number * Plnrate;
				AddTransferPlntoEur(number, amountEUR);
				return RedirectToAction("Index", "Home");
			}
		}
	}
}
