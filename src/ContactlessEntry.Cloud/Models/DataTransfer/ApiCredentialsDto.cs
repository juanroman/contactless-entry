using System.ComponentModel.DataAnnotations;

namespace ContactlessEntry.Cloud.Models.DataTransfer
{
    /// <summary>
    /// Defines an API Credentials DTO.
    /// </summary>
    public class ApiCredentialsDto
    {
        /// <summary>
        /// The API Key registered to use the service.
        /// </summary>
        [Required]
        public string ApiKey { get; set; }

        /// <summary>
        /// The API Secret registered to use the service.
        /// </summary>
        [Required]
        public string ApiSecret { get; set; }
    }
}

