using System.ComponentModel.DataAnnotations;

namespace ContactlessEntry.Cloud.Models.DataTransfer
{
    /// <summary>
    /// Defines the model for an Access DTO.
    /// </summary>
    public class RequestAccessDto
    {
        /// <summary>
        /// The unique door identifier for which access is being requested for.
        /// </summary>
        [Required]
        public string DoorId { get; set; }

        /// <summary>
        /// The person unique identifier that is requesting access.
        /// </summary>
        [Required]
        public string PersonId { get; set; }

        /// <summary>
        /// The temperature reading expressed in °C.
        /// </summary>
        [Required]
        [Range(1, 50)]
        public double Temperature { get; set; }
    }
}
