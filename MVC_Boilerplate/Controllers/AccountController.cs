using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Boilerplate.Models.Db.Account;
using MVC_Boilerplate.Models.View.Account;

namespace MVC_Boilerplate.Controllers
{
    public class AccountController : Controller
    {
        // Zaleśności do logowania i rejestracji użytkowników
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterView result)
        {
            // Sprawdzamy czy walidacja na modelu jest pozytywna
            if (ModelState.IsValid)
            {
                // Tworzymy użytkownika na podstawie przesłanych danych
                var user = new User() { UserName = result.Login, Email = result.Email };
                var callback = await _userManager.CreateAsync(user, result.Password);

                if (callback.Succeeded)
                {
                    // Logujemy użytkownika z ustawieniem aby nie był zapamiętany za pierwszym razem
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in callback.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(result);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginView result)
        {
            // Sprawdzanie walidacji modelu
            if (ModelState.IsValid)
            {
                // Logowanie użytkownika
                var callback = await _signInManager.PasswordSignInAsync(result.Login, result.Password, false, false);
                if (callback.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Nie można się zalogować!");
                }

            }
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
    }
}