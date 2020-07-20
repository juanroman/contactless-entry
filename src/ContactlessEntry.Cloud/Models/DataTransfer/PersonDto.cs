using System.ComponentModel.DataAnnotations;

namespace ContactlessEntry.Cloud.Models.DataTransfer
{
    public class PersonDto
    {
        [Required]
        public string PersonId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Url]
        public string FaceUrl { get; set; }
    }
}
