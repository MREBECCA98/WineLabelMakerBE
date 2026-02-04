using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace WineLabelMakerBE.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
