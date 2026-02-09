namespace Voyage.Models.DTO
{
    public class PermissionDTO
    {
        public string PermissionKey { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;

        //display in reverse specificity. ex. User Permission page: checkbox checked - inherited from Team Permissions (team)
        public string InheritsFrom { get; set; } = string.Empty; 

        public bool IsEnabled { get; set; } = false;
    }
}
