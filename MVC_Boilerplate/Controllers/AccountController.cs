using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Boilerplate.Models.Db.Account;
using MVC_Boilerplate.Models.View.Account;

namespace MVC_Boilerplate.Controllers
{
    public class AccountController : Controller
    {
        // Zaleśności do logowania i rejestracji użytkowników
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
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

        // Role

        [HttpGet]
        public async Task<IActionResult> RolesList()
        {
            var roles = await _roleManager.Roles?.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleView result)
        {
            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var role = new IdentityRole();
            role.Name = result.Name;
            await _roleManager.CreateAsync(role);

            return RedirectToAction("RolesList");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("RolesList");
            }

            var role = await _roleManager.FindByIdAsync(Id);

            await _roleManager.DeleteAsync(role);

            return RedirectToAction("RolesList");
        }

        // Użytkownicy

        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserToRole(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("UsersList");
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return RedirectToAction("UsersList");
            }

            ViewData["User"] = user;
            ViewData["Roles"] = await _roleManager.Roles?.ToListAsync();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddUserToRole(RoleToUserView result)
        {
            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var user = await _userManager.FindByIdAsync(result.userId);
            var role = await _roleManager.FindByIdAsync(result.roleId);

            await _userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction("UsersList");
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("UsersList");
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return RedirectToAction("UsersList");
            }

            var roles = await _userManager.GetRolesAsync(user);

            ViewData["roles"] = roles;
            ViewData["user"] = user;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRoleFromUser(RoleToUserView result)
        {
            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var user = await _userManager.FindByIdAsync(result.userId);
            var role = await _roleManager.FindByNameAsync(result.roleId);

            await _userManager.RemoveFromRoleAsync(user, role.Name);

            return RedirectToAction("UserDetails", new { Id = user.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
    }
}