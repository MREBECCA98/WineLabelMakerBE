using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Tabella messaggi per poter dialogare tra utente e admin riguardo una specifica richiesta
//Foreign Key verso Request e User (1 -> N),
//una richiesta può avere più messaggi ed un utente può inviare più messaggi (FK nella tabella dei molti)
namespace WineLabelMakerBE.Models.Entity
{
    public class Message
    {
        [Key]
        public Guid IdMessage { get; set; } //Primary Key

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; } //Testo del messaggio 

        public string? ImageUrl { get; set; } //Immmagine dell'etichetta inviata come messaggio (opzionale)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; //Timestamp di creazione

        //FK Request
        [Required]
        public Guid IdRequest { get; set; }
        [ForeignKey(nameof(IdRequest))]
        public Request Request { get; set; }

        //FK User
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
