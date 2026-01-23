using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Services;
using Voyage.Utilities;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Controllers
{
    public class CompanyController : Controller
    {
        private AccountBLL _accountBLL;
   

        public CompanyController(AccountBLL accountBLL)
        {
            _accountBLL = accountBLL;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register Company";

            var vm = new RegisterVM()
            {
                IsCompanyRegistration = true,
                CompanyId = 0,
                IsRenderedPartial = false
            };

            return View("~/Views/User/Register.cshtml", vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateHeaderAntiForgeryToken]
        public async Task RegisterCompany([FromBody] RegistrationDetailsDTO details)
        {
            await Task.Delay(10);
            var response = await _accountBLL.Register(details);
            //return Json(response);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            return Json(await CheckUsernameExists(username));
        }
    }
}
