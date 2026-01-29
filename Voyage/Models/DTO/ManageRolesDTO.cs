namespace Voyage.Models.DTO
{
    public class ManageRolesDTO
    {
        public ManageRolesDTO()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }
        public int RoleId { get; set; }
        public int? CompanyId { get; set; }
    }
}
