//Stato del ciclo di vita di una richiesta (in attesa, in corso, preventivo, pagamento confermato, completata, rifiutata)
//Utilizzo di enum per definire un insieme di costanti denominate
namespace WineLabelMakerBE.Models.Entity
{
    public enum RequestStatus
    {
        Pending,
        InProgress,
        QuoteSent,
        PaymentConfirmed,
        Completed,
        Rejected
    }
}
