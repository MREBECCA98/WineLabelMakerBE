//Stato del ciclo di vita di una richiesta (in attesa, in corso, preventivo, pagamento confermato, completata, rifiutata)
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
