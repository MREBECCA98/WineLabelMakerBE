using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Table of requests made by users for creating wine labels
//Foreign key to User (1 -> N)
//A user can make multiple requests (FK on the "many" side)

namespace WineLabelMakerBE.Models.Entity
{
    public class Request
    {
        [Key]
        public Guid IdRequest { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedByUserId { get; set; }

        //FK User
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        //Collezione di messaggi associati alla richiesta
        //public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
