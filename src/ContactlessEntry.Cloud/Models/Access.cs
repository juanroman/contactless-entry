using System;

namespace ContactlessEntry.Cloud.Models
{
    public class Access
    {
        public string DoorId { get; set; }

        public string PersonId { get; set; }

        public double Temerature { get; set; }

        public bool Granted { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
