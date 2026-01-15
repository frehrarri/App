using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Voyage.Data;
using Voyage.Utilities;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Business
{
    public class HrBLL
    {
        private readonly HrDAL _hrDAL;

        public HrBLL(HrDAL hrDAL)
        {
            _hrDAL = hrDAL;
        }

        public async Task SaveRoles(List<string> roles)
        {
            IgnoreAddDefaultRoles(ref roles);
            List<IdentityRole> rolesToSave = roles.Select(r => new IdentityRole(r)).ToList();
            await _hrDAL.SaveRoles(rolesToSave);
        }

        public async Task SaveDepartments(List<string> departments)
        {
            await _hrDAL.SaveDepartments(departments);
        }

        public async Task SavePermissions(List<string> permissions)
        {
            await _hrDAL.SavePermissions(permissions);
        }

        public async Task SaveTeams(List<string> teams)
        {
            await _hrDAL.SaveTeams(teams);
        }

        private void IgnoreAddDefaultRoles(ref List<string> roles)
        {
            List<string> rolesToCompare = roles.Select(r => r.ToUpper()).ToList();
            List<string> defaultRoles = Enum.GetNames<Constants.DefaultRoles>().ToList();

            foreach (var role in defaultRoles)
            {
                if (rolesToCompare.Equals(role.ToUpper()))
                    roles.Remove(role);
            }
        }
    }
}
