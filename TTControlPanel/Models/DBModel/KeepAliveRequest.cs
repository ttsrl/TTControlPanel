using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public class KeepAliveRequest
    {
        private DateTime _timestamp = DateTime.Now;

        public int Id { get; set; }
        public int Response { get; set; }
        public int DurationRequest { get; set; }
        public bool Inizialize { get; set; }
        public DateTime Timestamp { get => _timestamp; set => _timestamp = DateTime.Now; }
    }
}
