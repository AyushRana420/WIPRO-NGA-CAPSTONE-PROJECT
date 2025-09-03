using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fracto.Backend.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }

        [Required, StringLength(50)]
        public string? username { get; set; }

        [Required]
        public string? password { get; set; } // Store hashed password

        [StringLength(20)]
        public string? role { get; set; } // "User" or "Admin"

        [StringLength(255)]
        public string? profileImagePath { get; set; }

        // Relationships
        public ICollection<Appointment>? appointments { get; set; }
        public ICollection<Rating>? ratings { get; set; }
    }

    //Sample data
    public static class UserSampleData
    {
        public static List<User> GetSampleUsers()
        {
            return new List<User>
            {
                new User
                {
                    username = "john_doe",
                    password = "hashed_password_1",
                    role = "User",
                    profileImagePath = "uploads/users/user1.jpg",
                    appointments = new List<Appointment>(),
                    ratings = new List<Rating>()
                },
                new User
                {
                    username = "jane_smith",
                    password = "hashed_password_2",
                    role = "Admin",
                    profileImagePath = "uploads/users/user2.jpg",
                    appointments = new List<Appointment>(),
                    ratings = new List<Rating>()
                }
            };
        }
    }
}