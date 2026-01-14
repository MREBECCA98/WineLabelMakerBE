
//Stato del ciclo di vita di una richiesta (in attesa, in corso, completata, rifiutata)
//Utilizzo di enum per definire un insieme di costanti denominate
namespace WineLabelMakerBE.Models.Entity
{
    public enum RequestStatus
    {
        Pending,
        InProgress,
        Completed,
        Rejected
    }
}
