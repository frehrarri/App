using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.User;
using Voyage.Services;
using Voyage.Utilities;
using static Voyage.Utilities.CustomAttributes;


namespace Voyage.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private EmailService _emailService;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public IActionResult Register()
        {
            return View();
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
            PasswordReset vm = new PasswordReset { Token = token, Email = email };
            return View("~/Views/User/ResetPassword.cshtml", vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(PasswordReset model)
        {
            if (String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Token))
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
                return RedirectToAction("Login");

            //foreach (var error in result.Errors)
            //    ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationDetails details)
        {
            Response response = new Response();

            if (details == null)
            {
                response.Message = Constants.BadRequest;
                response.StatusCode = HttpStatusCode.BadRequest;
                return Json(response);
            }

            response = await Validate(details);
       
            if (response.ErrorMessages.Any())
            {
                response.Message = Constants.BadRequest;
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            //passes validation so we create a new user
            var user = new AppUser
            {
                UserName = details.Username.Trim(),
                Email = details.Email.Trim(),
                FirstName = details.FirstName.Trim(),
                MiddleName = details.MiddleName.Trim(),
                LastName = details.LastName.Trim(),
                PhoneNumber = details.Phone.ToString(),
                StreetAddress = details.StreetAddress.Trim(),
                UnitNumber = details.UnitNumber,
                City = details.City.Trim(),
                State = details.State.Trim(),
                Country = details.Country.Trim(),
                PostalCode = details.ZipCode
            };

            IdentityResult result = await _userManager.CreateAsync(user, details.Password);
            if (result.Succeeded)
            {
                //assign role
                await _userManager.AddToRoleAsync(user, Enum.GetName(Constants.Roles.Unassigned));

                //sign in the user immediately
                await _signInManager.SignInAsync(user, isPersistent: false);

                response.StatusCode = HttpStatusCode.OK;
                response.Message = Constants.RegistrationSuccessful;
                response.RedirectURL = Url.Action("Home", "Website");
            }
            else
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = Constants.BadRequest;
            }

             return Json(response);
        }


        public async Task<bool> UsernameExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            var user = await _userManager.FindByNameAsync(username);
            bool exists = user != null;

            return exists;
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            return Json(await UsernameExists(username));
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> LoginUser([FromBody] Login login)
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
                //assign role "unassigned" if existing user didn't have one assigned
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.ToList().Any() == false)
                {
                    await _userManager.AddToRoleAsync(user, Enum.GetName(Constants.Roles.Unassigned));
                }
                await _signInManager.RefreshSignInAsync(user);

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

        public async Task<Response> Validate(RegistrationDetails details)
        {
            Response response = new Response();

            await ValidateUsername(details, response);
            await ValidateName(details, response);
            await ValidatePassword(details, response);
            await ValidatePhone(details, response);
            await ValidateEmail(details, response);
            await ValidateAddress(details, response);

            return response;
        }

        private async Task ValidateUsername(RegistrationDetails details, Response response)
        {
            if (string.IsNullOrEmpty(details.Username))
            {
                response.ErrorMessages.Add(Constants.ErrUsernameIsReq);
            }

            if (details.Username != null && details.Username.Length < 5)
            {
                response.ErrorMessages.Add(Constants.ErrUsernameMinLen);
            }

            if (details.Username != null && details.Username.Length > 20)
            {
                response.ErrorMessages.Add(Constants.ErrUsernameMaxLen);
            }

            if (await UsernameExists(details.Username))
            {
                response.ErrorMessages.Add(Constants.ErrUsernameExists);
            }
        }

        private async Task ValidateName(RegistrationDetails details, Response response)
        {
            if (string.IsNullOrEmpty(details.FirstName))
            {
                response.ErrorMessages.Add(Constants.ErrFirstNameReq);
            }

            if (string.IsNullOrEmpty(details.LastName))
            {
                response.ErrorMessages.Add(Constants.ErrLastNameReq);
            }
        }

        private async Task ValidatePassword(RegistrationDetails details, Response response)
        {
            if (string.IsNullOrEmpty(details.Password) || details.Password.Length < 8)
            {
                response.ErrorMessages.Add(Constants.ErrPassMinLen);
            }

            if (details.Password != null && details.Password.Length > 20)
            {
                response.ErrorMessages.Add(Constants.ErrPassMaxLen);
            }
        }

        private async Task ValidatePhone(RegistrationDetails details, Response response)
        {

            if (details.PhoneAreaCode == 0)
            {
                response.ErrorMessages.Add(Constants.ErrPhoneAreaCodeIsReq);
            }

            if (details.PhoneAreaCode.ToString().Length > 5)
            {
                response.ErrorMessages.Add(Constants.ErrPhoneAreaCodeMaxLen);
            }

            if (details.Phone == 0)
            {
                response.ErrorMessages.Add(Constants.ErrPhoneNumReq);
            }

            if (details.Phone.ToString().Length > 10)
            {
                response.ErrorMessages.Add(Constants.ErrPhoneNumMaxLen);
            }

            if (details.Phone.ToString().Length < 3)
            {
                response.ErrorMessages.Add(Constants.ErrPhoneNumMinLen);
            }

        }

        private async Task ValidateEmail(RegistrationDetails details, Response response)
        {
            var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

            if (string.IsNullOrEmpty(details.Email) || !System.Text.RegularExpressions.Regex.IsMatch(details.Email, emailRegex))
            {
                response.ErrorMessages.Add(Constants.ErrInvalidEmail);
            }
        }

        private async Task ValidateAddress(RegistrationDetails details, Response response)
        {
            if (string.IsNullOrEmpty(details.StreetAddress))
            {
                response.ErrorMessages.Add(Constants.ErrStreetAddrReq);
            }

            if (string.IsNullOrEmpty(details.City))
            {
                response.ErrorMessages.Add(Constants.ErrCityReq);
            }

            if (string.IsNullOrEmpty(details.State))
            {
                response.ErrorMessages.Add(Constants.ErrStateReq);
            }

            if (string.IsNullOrEmpty(details.Country))
            {
                response.ErrorMessages.Add(Constants.ErrCountryReq);
            }

            if (details.ZipCode == 0)
            {
                response.ErrorMessages.Add(Constants.ErrPostalCode);
            }

            if (details.ZipCode.ToString().Length < 3)
            {
                response.ErrorMessages.Add(Constants.ErrPostCodeMinLen);
            }

            if (details.ZipCode.ToString().Length > 10)
            {
                response.ErrorMessages.Add(Constants.ErrPostCodeMaxLen);
            }

        }























    }


}
