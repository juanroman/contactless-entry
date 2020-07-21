using System.ComponentModel.DataAnnotations;

namespace ContactlessEntry.Cloud.Models.DataTransfer
{
    public class NewPersonDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Url]
        public string FaceUrl { get; set; }
    }
}
