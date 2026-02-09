namespace Voyage.Models.App
{
    public class PermissionsVM
    {
        public List<PermissionVM> Permissions { get; set; } = new List<PermissionVM>();
        public bool AccessGranted { get; set; }
    }
}
