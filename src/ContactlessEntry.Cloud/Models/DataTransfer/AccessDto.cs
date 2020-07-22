using System;

namespace ContactlessEntry.Cloud.Models.DataTransfer
{
    public class AccessDto
    {
        public string DoorId { get; set; }

        public string PersonId { get; set; }

        public double Temperature { get; set; }

        public bool Granted { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
