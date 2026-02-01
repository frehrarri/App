namespace Voyage.Models.DTO
{
    public class ManageRolesDTO : BaseClass
    {
        public ManageRolesDTO()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }
        public int RoleId { get; set; }
        public int? CompanyId { get; set; }
        public int DbChangeAction { get; set; }
    }
}
