using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Voyage.Data;
using Voyage.Models.DTO;
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

        public async Task<List<ManagePersonnelDTO>> GetPersonnel()
        {
            return await _hrDAL.GetPersonnel();
        }

        public async Task<List<ManageRolesDTO>> GetRoles()
        {
            return await _hrDAL.GetRoles();
        }

        public async Task<List<ManageDepartmentsDTO>> GetDepartments()
        {
            return await _hrDAL.GetDepartments();
        }

        public async Task<List<ManageTeamsDTO>> GetTeams()
        {
            return await _hrDAL.GetTeams();
        }

        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrDAL.GetPermissions();
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
