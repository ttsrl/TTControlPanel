using System;
using System.Collections.Generic;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class Order
    {
        private DateTime? timestamp;

        public int Id { get; set; }
        public int Number { get; set; }
        public Client Client { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Invoice Invoice { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
