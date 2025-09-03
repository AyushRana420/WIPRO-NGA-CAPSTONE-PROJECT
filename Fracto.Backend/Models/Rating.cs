using System.ComponentModel.DataAnnotations;

namespace Fracto.Backend.Models
{
    public class Rating
    {
        [Key]
        public int ratingId { get; set; }

        [Required]
        public int doctorId { get; set; }
        public Doctor? doctor { get; set; }

        [Required]
        public int userId { get; set; }
        public User? user { get; set; }

        [Required]
        [Range(1, 5)]
        public int value { get; set; } // 1–5 rating
    }
}