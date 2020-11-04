using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class WorkingItem
    {
        private DateTime? timestamp;

        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public User Operator { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
