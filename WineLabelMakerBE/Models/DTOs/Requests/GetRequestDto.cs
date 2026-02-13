using System.ComponentModel.DataAnnotations;

//DTOs (Data Transfer Objects) are used to transfer data between the client and server
//They expose only the data needed for a specific operation,
//hiding sensitive or unnecessary information
//In this case, the "GetRequestDto" represents the data of a request
//that will be sent from the server to the client,
//including details such as the request ID, the user's name and email,
//the request description, status, and creation date

namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class GetRequestDto
    {
        public Guid IdRequest { get; set; }

        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserSurname { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
