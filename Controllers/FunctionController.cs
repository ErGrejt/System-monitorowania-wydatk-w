using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Controllers;
using freecurrencyapi;
using freecurrencyapi.Helpers;
using Org.BouncyCastle.Security;

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
                Jedzenie newFood = new Jedzenie
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency,
                    UserID = (int)HttpContext.Session.GetInt32("UserId")
            };
                _context.Jedzenie.Add(newFood);
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
                Zdrowie newHealth = new Zdrowie
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency,
                    UserID = (int)HttpContext.Session.GetInt32("UserId")

                };
                _context.Zdrowie.Add(newHealth);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        //Dodanie do tabeli Zachcianki
        public IActionResult AddZach(FormData model)
        {
            if (ModelState.IsValid)
            {
                Zachcianki newZach = new Zachcianki
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency,
                    UserID = (int)HttpContext.Session.GetInt32("UserId")

                };

                _context.Zachcianki.Add(newZach);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        //Dodanie do tabeli Przelewy
        public IActionResult AddPrzelew(FormData model)
        {
            if (ModelState.IsValid)
            {
                Przelewy newPrzelew = new Przelewy
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency,
                    Kierunek = model.Who,
                    UserID = (int)HttpContext.Session.GetInt32("UserId")

                };
                _context.Przelewy.Add(newPrzelew);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        //Dodanie do tabeli Saldo
        public IActionResult AddSaldo(FormData model)
        {
            Saldo newSaldo = new Saldo
            {
                PLN = model.PLN,
                EURO = model.EURO,
                UserID = (int)HttpContext.Session.GetInt32("UserId")

            };
            _context.Saldo.Add(newSaldo);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");

        }
        //Funkcje dodawania
        public IActionResult AddPrzelewEurToPln(decimal euramount, decimal plnafterexchange)
        {
            Przelewy newPrzelewWymianaEurMinus = new Przelewy
            {
                Nazwa = "Wymiana Eur->Pln",
                Cena = euramount,
                Waluta = 2,
                Kierunek = 1,
                UserID = (int)HttpContext.Session.GetInt32("UserId")
            };
            Przelewy newPrzelewWymianaPlnPlus = new Przelewy
            {
                Nazwa = "Wymiana Eur->Pln",
                Cena = plnafterexchange,
                Waluta = 1,
                Kierunek = 2,
                UserID = (int)HttpContext.Session.GetInt32("UserId")
            };
            _context.Przelewy.Add(newPrzelewWymianaEurMinus);
            _context.SaveChanges();
            _context.Przelewy.Add(newPrzelewWymianaPlnPlus);
            _context.SaveChanges();


            return RedirectToAction("Index", "Home");
        }
        public IActionResult AddPrzelewPlnToEur(decimal plnamount, decimal eurafterexchange)
        {
            Przelewy newPrzelewWymianaPlnMinus = new Przelewy
            {
                Nazwa = "Wymiana Pln->Eur",
                Cena = plnamount,
                Waluta = 1,
                Kierunek = 1,
                UserID = (int)HttpContext.Session.GetInt32("UserId")
            };
            Przelewy newPrzelewWymianaEurPlus = new Przelewy
            {
                Nazwa = "Wymiana Pln->Eur",
                Cena = eurafterexchange,
                Waluta = 2,
                Kierunek = 2,
                UserID = (int)HttpContext.Session.GetInt32("UserId")
            };
            _context.Przelewy.Add(newPrzelewWymianaPlnMinus);
            _context.SaveChanges();
            _context.Przelewy.Add(newPrzelewWymianaEurPlus);
            _context.SaveChanges();


            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult AddUser(Users model)
        {
            if(ModelState.IsValid)
            {
                Users newUser = new Users
                {
                    Email = model.Email,
                    Password = model.Password
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Login", "Home");
            }

            return RedirectToAction("Register","Home");
        }
        [HttpPost]
        public IActionResult CheckUser(UserLogin model)
        {
            if(ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if(user != null)
                {

                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return RedirectToAction("Index","Home");
                }
            }

            return RedirectToAction("Login", "Home");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login","Home");
        }

        [HttpPost]
        public IActionResult AddTransaction(FormData model)
        {

            if (ModelState.IsValid)
            {
                if (model.Category == 1)
                {
                    AddPrzelew(model);
                }
                else if (model.Category == 2)
                {
                    AddFood(model);
                }
                else if (model.Category == 3)
                {   
                    AddHealth(model);
                }
                else if (model.Category == 4)
                {
                    AddZach(model);
                }
            }
            return RedirectToAction("index", "Home");
        }

        public IActionResult AddSaldoTransaction(FormData model)
        {
            AddSaldo(model);
            return RedirectToAction("Form", "Home");
        }
        [HttpPost]
        public IActionResult DodajWymianeEur(string AmountEur)
        {
            decimal numer = Convert.ToDecimal(AmountEur);
            //Klucz freecurrency
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            //Wyciąganie kursu eur -> pln
            string kursEURtoPLN = fx.Latest("EUR", "PLN");
            kursEURtoPLN = kursEURtoPLN.Substring(15, 12);
            kursEURtoPLN = kursEURtoPLN.Remove(1, 1).Insert(1, ",").Remove(4, 8);
            decimal kurseuro = Convert.ToDecimal(kursEURtoPLN);
            decimal amountPLN = numer * kurseuro;
            AddPrzelewEurToPln(numer, amountPLN);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult DodajWymianePln(string AmountPLN)
        {
            decimal numer = Convert.ToDecimal(AmountPLN);
            //Klucz freecurrency
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            //Wyciąganie kursu eur -> pln
            string kursPlnToEur = fx.Latest("PLN", "EUR");
            kursPlnToEur = kursPlnToEur.Substring(15, 12);
            kursPlnToEur = kursPlnToEur.Remove(1, 1).Insert(1, ",").Remove(4, 8);
            decimal kurspln = Convert.ToDecimal(kursPlnToEur);
            decimal amountEUR = numer * kurspln;
           AddPrzelewPlnToEur(numer, amountEUR);

            return RedirectToAction("Index", "Home");
        }
    }
}
