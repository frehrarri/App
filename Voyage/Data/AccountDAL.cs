using AngleSharp.Css;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.DTO;
using Voyage.Utilities;

namespace Voyage.Data
{
    public class AccountDAL
    {
        private _AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager; 
        private ILogger<AccountDAL> _logger;

        public AccountDAL(_AppDbContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountDAL> logger)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Response> Register(RegistrationDetailsDTO details)
        {
            Response response = new Response();
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (details == null)
                {
                    response.Message = Constants.BadRequest;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
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
                if (isNewCompany)
                {
                    companyId = await GetNextCompanyId();

                    Company company = new Company();
                    company.CompanyId = companyId;
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
                //register employee to an existing company
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
                    EmployeeId = details.IsCompanyRegistration ? 1 : await GetNextEmployeeId(companyId) 
                };

                IdentityResult result = await _userManager.CreateAsync(user, details.Password);
                if (result.Succeeded)
                {
                    //assign default roles
                    if (isNewCompany)
                        await AssignRoleToUser(user, (int)Constants.DefaultRoles.Principal, companyId);
                    else
                        await AssignRoleToUser(user, (int)Constants.DefaultRoles.Unassigned, companyId);

                    //sign in the user immediately
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = Constants.RegistrationSuccessful;
                    response.RedirectURL = "/Website/Home";
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = Constants.BadRequest;
                }

                await transaction.CommitAsync();
                return response;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                _logger.LogError(e, "error: register user");
                return null!;
            }
        }

        private async Task<int> GetNextCompanyId()
        {
            return await _db.Companies.MaxAsync(c => c.CompanyId) + 1;
        }

        private async Task AssignRoleToUser(AppUser user, int roleId, int companyId)
        {
            try
            {
                // Always fetch the tracked Role instance
                var role = await _db.CompanyRoles
                    .Where(r => r.RoleId == roleId)
                    .FirstOrDefaultAsync();

                if (role == null)
                    throw new InvalidOperationException($"Role not found for company {companyId}");

                // Check for duplicate assignment
                bool alreadyAssigned = await _db.IndividualUserRoles.AnyAsync(iur =>
                    iur.CompanyId == companyId &&
                    iur.EmployeeId == user.EmployeeId &&
                    iur.RoleKey == role.RoleKey);

                if (alreadyAssigned)
                    return;

                var userRole = new IndividualUserRole
                {
                    IndivUserRoleKey = Guid.NewGuid(),
                    CompanyId = companyId,
                    EmployeeId = user.EmployeeId,
                    RoleId = role.RoleId,
                    RoleKey = role.RoleKey
                };

                await _db.IndividualUserRoles.AddAsync(userRole);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error: AssignRoleToUser");
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

        private async Task<int> GetNextRoleId(int companyId)
        {
            // Find the current max RoleId for this company
            int? maxRoleId = await _db.CompanyRoles
                .Where(r => r.CompanyId == companyId)
                .MaxAsync(r => (int?)r.RoleId);

            // If no roles exist, start at 1
            return (maxRoleId ?? 0) + 1;
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

    
        public async Task<bool> CheckUsernameExists(string username)
        {
            return await UsernameExists(username);
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
