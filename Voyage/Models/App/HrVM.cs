namespace Voyage.Models.App
{
    public class HrVM
    {
        public HrVM() 
        { 
            ManagePersonnelVM = new ManagePersonnelVM();
        }

        public ManagePersonnelVM ManagePersonnelVM { get; set; }
    }
}
