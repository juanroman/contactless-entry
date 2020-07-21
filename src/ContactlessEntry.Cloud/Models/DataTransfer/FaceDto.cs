using System.ComponentModel.DataAnnotations;

namespace ContactlessEntry.Cloud.Models.DataTransfer
{
    public class FaceDto
    {
        [Required]
        [Url]
        public string FaceUrl { get; set; }
    }
}
