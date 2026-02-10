using Microsoft.AspNetCore.Mvc;
using Voyage.Data;
using Voyage.Models.DTO;

namespace Voyage.Business
{
    public class PermissionsBLL
    {
        private readonly PermissionsDAL _permissionsDAL;
        public PermissionsBLL(PermissionsDAL permissionsDAL)
        {
            _permissionsDAL = permissionsDAL;
        }

        #region Get Methods

        public async Task<PermissionsDTO> GetRolePermissions(int companyId, string roleKey)
        {
            return await _permissionsDAL.GetRolePermissions(companyId, roleKey);
        }

        public async Task<PermissionsDTO> GetDeptPermissions(int companyId, string deptKey)
        {
            return await _permissionsDAL.GetDeptPermissions(companyId, deptKey);
        }

        public async Task<PermissionsDTO> GetTeamPermissions(int companyId, string teamKey)
        {
            return await _permissionsDAL.GetTeamPermissions(companyId, teamKey);
        }

        public async Task<PermissionsDTO> GetUserPermissions(int companyId, string userKey)
        {
            return await _permissionsDAL.GetUserPermissions(companyId, userKey);
        }


        #endregion


        #region Save Methods

        public async Task SetDefaultRolePermissions(PermissionsDTO dto)
        {
            await _permissionsDAL.SetDefaultRolePermissions(dto);
        }

        public async Task SetPermissions(PermissionsDTO dto)
        {
            await _permissionsDAL.SetPermissions(dto);
        }

        #endregion


    }
}
