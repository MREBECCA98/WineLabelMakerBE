using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Tabella delle richieste fatte dagli utenti per la creazione di etichette di vino
//Foreign Key verso User (1 -> N),
//un utente può fare più richieste (FK nella tabella dei molti)
namespace WineLabelMakerBE.Models.Entity
{
    public class Request
    {
        [Key]
        public Guid IdRequest { get; set; } //Primary Key

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; } //Descrizione della richiesta

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending; //Stato della richiesta, di default in attesa

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; //Timestamp di creazione

        public DateTime? UpdatedAt { get; set; } //Timestamp dell'ultimo aggiornamento 

        public string? UpdatedByUserId { get; set; } //ID dell'utente che ha fatto l'ultimo aggiornamento

        //FK User
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        //Collezione di messaggi associati alla richiesta
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
