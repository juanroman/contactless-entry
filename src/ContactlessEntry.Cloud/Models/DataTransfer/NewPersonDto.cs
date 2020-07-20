using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
