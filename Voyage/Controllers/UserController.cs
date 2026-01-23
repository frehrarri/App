using AngleSharp.Css;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Services;
using Voyage.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Voyage.Utilities.CustomAttributes;


namespace Voyage.Controllers
{
    public class UserController : Controller
    {
        private AccountBLL _accountBLL;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private EmailService _emailService;

        public UserController(AccountBLL accountBLL, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, EmailService emailService)
        {
            _accountBLL = accountBLL;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        
        [AllowAnonymous]
        public IActionResult Register(int companyId)
        {
            ViewData["Title"] = "Register";

            var vm = new RegisterVM() 
            { 
                IsCompanyRegistration = false, 
                CompanyId = companyId ,
                IsRenderedPartial = false
            };

            return View("~/Views/User/Register.cshtml", vm);
        }

        [HttpGet]
        public IActionResult RegisterUserPartial(int companyId)
        {
            ViewData["Title"] = "Register";

            var vm = new RegisterVM()
            {
                IsCompanyRegistration = false,
                CompanyId = companyId,
                IsRenderedPartial = true
            };

            return PartialView("~/Views/User/Register.cshtml", vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string toEmail)
        {
            if (string.IsNullOrEmpty(toEmail))
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(toEmail);

            // IMPORTANT: Always return success (prevents user enumeration)
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok();

            if (user.Email == null)
            {
                return BadRequest();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "User", new { token, email = user.Email }, Request.Scheme);

            Email email = new Email();
            email.FromEmail = Constants.SystemEmail;
            email.FromName = "Voyage";
            email.ToEmail = user.Email;
            email.ToName = user.FirstName;
            email.Subject = "Reset your password";
            email.Body = $"Click here to reset your password: <a href='{callbackUrl}'>Reset</a>";

            await _emailService.Send(email);

            return Ok();
        }

        //called from email generated from ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            PasswordResetDTO vm = new PasswordResetDTO { Token = token, Email = email };
            return View("~/Views/User/ResetPassword.cshtml", vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(PasswordResetDTO model)
        {
            if (String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Token))
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("Login");

            if (!String.IsNullOrEmpty(model.Password))
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                    return RedirectToAction("Login");
            }


            return View(model);
        }

 

        [HttpPost]
        [AllowAnonymous]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationDetailsDTO details)
        {
            var response = await _accountBLL.Register(details);
            return Json(response);
        }

        

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(Enumerable.Empty<object>());

            var users = await _userManager.Users
                .Where(u => (u.UserName ?? "").Contains(query) || (u.Email ?? "").Contains(query))
                .OrderBy(u => u.UserName)
                .Take(5)
                .Select(u => new {
                    id = u.Id,
                    displayName = u.UserName,
                    email = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            bool exists = await _accountBLL.CheckUsernameExists(username);
            return Json(exists);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> LoginUser([FromBody] LoginDTO login)
        {
            Response response = new Response();

            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = Constants.BadRequest;
                response.ErrorMessages.Add(Constants.ErrBadUsernameOrPassword);
                return Json(response);
            }

            var user = await _userManager.FindByEmailAsync(_userManager.NormalizeEmail(login.Email));
            if (user == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = Constants.BadRequest;
                response.ErrorMessages.Add(Constants.ErrBadUsernameOrPassword);
                return Json(response);
            }

            // This checks the password, locks out user if necessary, etc.
            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                response.RedirectURL = Url.Action("Home", "Website");
                response.StatusCode = HttpStatusCode.OK;
                response.Message = Constants.LoginSuccessful;
            }
            else if (result.IsLockedOut)
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                response.Message = Constants.TooManyLoginAttempts;
            }
            else
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.Message = Constants.InvalidLoginAttempt;
            }

            return Json(response);
        }

  

        























    }


}
