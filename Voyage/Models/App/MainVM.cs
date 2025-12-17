namespace Voyage.Models.App
{
    public class MainVM
    {
        public MainVM()
        {
            TicketsVM = new TicketsVM();
        }

        public TicketsVM? TicketsVM { get; set; }
    }
}
