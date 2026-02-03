namespace Voyage.Models
{
    public class GridControlVM
    {
        public GridControlVM() 
        {
            Headers = new List<string>();
            Rows = new List<GridRow>();
        }


        public string TextId { get; set; }
        public List<string> Headers { get; set; }
        public List<GridRow> Rows { get; set; }
    }

    public class GridRow()
    {
        public List<string> DataItems { get; set; } = new List<string>();
    }
}
