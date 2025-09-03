using System.ComponentModel.DataAnnotations;

namespace Fracto.Backend.Models
{
    public class Specialization
    {
        [Key]
        public int specializationId { get; set; }

        [Required, StringLength(100)]
        public string? specializationName { get; set; }

        // Relationships
        public ICollection<Doctor>? doctors { get; set; }
    }
}