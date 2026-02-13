using System.ComponentModel.DataAnnotations;

//DTOs (Data Transfer Objects) are used to transfer data between the client and server
//They expose only the data needed for a specific operation,
//hiding sensitive or unnecessary information
//In this case, the "CreateRequestDto" is used to send the data required to create a
//new request, exposing only the description

namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class CreateRequestDto
    {
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }
    }
}
