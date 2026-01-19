using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
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
        private ILogger<AppUser> _logger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private EmailService _emailService;
        private _AppDbContext _db;


        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailService, _AppDbContext db, ILogger<AppUser> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _db = db;
            _logger = logger;
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
        public IActionResult RegisterCompany()
        {
            ViewData["Title"] = "Register Company";

            var vm = new RegisterVM()
            { 
                IsCompanyRegistration = true, 
                CompanyId = 0 
            };

            return View("~/Views/User/Register.cshtml", vm);
        }

        [AllowAnonymous]
        public IActionResult RegisterUser(int companyId)
        {
            ViewData["Title"] = "Register";

            var vm = new RegisterVM() 
            { 
                IsCompanyRegistration = false, 
                CompanyId = companyId 
            };

            return View("~/Views/User/Register.cshtml", vm);
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
        public async Task<IActionResult> Register([FromBody] RegistrationDetailsDTO details)
        {
            Response response = new Response();
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
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

                bool isNewCompany = details.IsCompanyRegistration;
                int companyId = details.Company.CompanyId;

                //this will be the Company Owner's registration because we add a new company to the company table.
                //need to create a separate registration where we register an employee to an company instead of adding a new one
                
                if (isNewCompany)
                {
                    Company company = new Company();
                    company.Name = details.Company.Name;
                    company.Email = details.Company.Email;
                    company.Phone = details.Company.Phone;
                    company.StreetAddress = details.Company.StreetAddress;
                    company.City = details.Company.City;
                    company.State = details.Company.State;
                    company.PostalCode = details.Company.PostalCode;
                    company.Region = details.Company.Region;
                    company.Country = details.Company.Country;

                    await _db.AddAsync(company);
                    await _db.SaveChangesAsync();

                    companyId = company.CompanyId;
                }
                else
                {
                    bool exists = await _db.Companies.AnyAsync(c => c.CompanyId == details.Company.CompanyId);
                    if (exists)
                        companyId = details.Company.CompanyId;
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
                    PostalCode = details.ZipCode,
                    CompanyId = companyId,
                    EmployeeId = details.IsCompanyRegistration ? 1 : await GetNextEmployeeId(companyId) ///how do i want to handle this for regular user registration?
                };

                IdentityResult result = await _userManager.CreateAsync(user, details.Password);
                if (result.Succeeded)
                {
                    //assign role
                    await _userManager.AddToRoleAsync(user, nameof(Constants.DefaultRoles.Unassigned));

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

                await transaction.CommitAsync();
                return Json(response);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                _logger.LogError(e, "error: register user");
                throw;
            }
        }

        private async Task<int> GetNextEmployeeId(int companyId)
        {
            var maxEmployeeId = await _db.Users
                .Where(u => u.CompanyId == companyId)
                .Select(u => (int?)u.EmployeeId)
                .MaxAsync();

            return (maxEmployeeId ?? 0) + 1;
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

        public async Task<bool> UsernameExists(string? username)
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
        [AllowAnonymous]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            return Json(await UsernameExists(username));
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
                //assign role "unassigned" if existing user didn't have one assigned
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.ToList().Any() == false)
                {
                    await _userManager.AddToRoleAsync(user, nameof(Constants.DefaultRoles.Unassigned));
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

        public async Task<Response> Validate(RegistrationDetailsDTO details)
        {
            Response response = new Response();

            await ValidateUsername(details, response);
            ValidateName(details, response);
            ValidatePassword(details, response);
            ValidatePhone(details, response);
            ValidateEmail(details, response);
            ValidateAddress(details, response);

            return response;
        }

        private async Task ValidateUsername(RegistrationDetailsDTO details, Response response)
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

        private void ValidateName(RegistrationDetailsDTO details, Response response)
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

        private void ValidatePassword(RegistrationDetailsDTO details, Response response)
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

        private void ValidatePhone(RegistrationDetailsDTO details, Response response)
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

        private void ValidateEmail(RegistrationDetailsDTO details, Response response)
        {
            var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

            if (string.IsNullOrEmpty(details.Email) || !System.Text.RegularExpressions.Regex.IsMatch(details.Email, emailRegex))
            {
                response.ErrorMessages.Add(Constants.ErrInvalidEmail);
            }
        }

        private void ValidateAddress(RegistrationDetailsDTO details, Response response)
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
