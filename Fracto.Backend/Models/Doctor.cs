using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fracto.Backend.Models
{
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int doctorId { get; set; }

        [Required, StringLength(100)]
        public string? name { get; set; }

        [Required]
        public int specializationId { get; set; }

        [Required, StringLength(100)]
        public string? city { get; set; }

        public double rating { get; set; }

        [StringLength(255)]
        public string? profileImagePath { get; set; }

        // This navigation property tells EF Core that a Doctor is related to one Specialization.
        public Specialization Specialization { get; set; } = null!;

        // Relationships
        public ICollection<Appointment>? appointments { get; set; }
        public ICollection<Rating>? ratings { get; set; }
    }
}