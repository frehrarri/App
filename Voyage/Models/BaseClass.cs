

namespace Voyage.Models
{
    public class BaseClass
    {
        public string ModifiedDate { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsLatest { get; set; }
        public bool IsActive { get; set; }

    }
}
