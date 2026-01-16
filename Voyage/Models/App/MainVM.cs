namespace Voyage.Models.App
{
    public class MainVM
    {
        public MainVM()
        {
            TicketsVM = new TicketsVM();
            HrVM = new HrVM();
        }

        public TicketsVM? TicketsVM { get; set; }
        public HrVM HrVM { get; set; }
    }
}
