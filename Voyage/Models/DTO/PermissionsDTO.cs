namespace Voyage.Models.DTO
{
    public class PermissionsDTO : BaseClass
    {
        public PermissionsDTO()
        {
            Permissions = new List<PermissionDTO>();
            AccessGranted = false;
        }

        public List<PermissionDTO> Permissions { get; set; }
        public bool AccessGranted { get; set; }
        public int CompanyId { get; set; }
        public string RoleKey { get; set; }
        public int RoleType { get; set; }
        public string DepartmentKey { get; set; }
        public string TeamKey { get; set; }
        public string UserKey { get; set; }
    }
}
