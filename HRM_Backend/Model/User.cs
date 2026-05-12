using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public int RoleId {  get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public DateTime ActionDate { get; set; }
    }
}
