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

        public async Task SetDefaultRolePermissions(PermissionsDTO dto)
        {
            await _permissionsDAL.SetDefaultRolePermissions(dto);
        }

        public async Task SetRolePermissions(PermissionsDTO dto)
        {
            await _permissionsDAL.SetRolePermissions(dto);
        }

        public async Task<PermissionsDTO> GetRolePermissions(int companyId, string roleKey)
        {
            return await _permissionsDAL.GetRolePermissions(companyId, roleKey);
        }

    }
}
