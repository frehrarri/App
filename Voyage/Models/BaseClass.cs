

namespace Voyage.Models
{
    public class BaseClass
    {
        public DateTime? ModifiedDate { get; set; } 
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool? IsLatest { get; set; }
        public bool? IsActive { get; set; }

    }
}
