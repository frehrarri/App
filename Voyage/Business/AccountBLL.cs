using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.DTO;
using Voyage.Services;

namespace Voyage.Business
{
    public class AccountBLL
    {
        AccountDAL _accountDAL;

        public AccountBLL(AccountDAL accountDAL)
        {
            _accountDAL = accountDAL;
        }

        public async Task<Response> Register(RegistrationDetailsDTO details)
        {
            return await _accountDAL.Register(details);
        }

        public async Task<bool> CheckUsernameExists(string username)
        {
            return await _accountDAL.CheckUsernameExists(username);
        }
    }
}
