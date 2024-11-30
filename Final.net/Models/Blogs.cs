using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final.net.Models
{
    [Table("Blogs")]
    public class Blogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageURL { get; set; }

        [Required]
        [StringLength(500)]
        public string LinkURL { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool Active { get; set; }

        public void UpdateTimestamp()
        {
            this.UpdatedAt = DateTime.Now;
        }
    }
}