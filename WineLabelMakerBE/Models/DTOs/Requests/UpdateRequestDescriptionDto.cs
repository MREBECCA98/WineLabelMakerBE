using System.ComponentModel.DataAnnotations;

//DTOs (Data Transfer Objects) are used to transfer data between the client and server.
//They expose only the data needed for a specific operation,
//hiding sensitive or unnecessary information
//In this case, the "UpdateRequestDescriptionDto" represents the data needed
//to update an existing request's description, accessible only to the user

namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class UpdateRequestDescriptionDto
    {
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }
    }
}
