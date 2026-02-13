using System.ComponentModel.DataAnnotations;
using WineLabelMakerBE.Models.Entity;

//DTOs (Data Transfer Objects) are used to transfer data between the client and server
//They expose only the data needed for a specific operation,
//hiding sensitive or unnecessary information.
//In this case, the "UpdateRequestStatusDto" represents the data needed
//to update an existing request's status.
//This status update can only be performed by an administrator

namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class UpdateRequestStatusDto
    {
        [Required]
        public RequestStatus Status { get; set; }
    }
}
