
//Lifecycle status of a request (pending, in progress, quote, payment confirmed, completed, rejected)
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
