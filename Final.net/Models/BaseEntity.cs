using System.ComponentModel.DataAnnotations;

namespace Final.net.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
