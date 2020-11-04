using System;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class Product
    {
        DateTime? timestamp;

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int VAT { get; set; }
        public double SellingPrice { get; set; }
        public double AveragePrice { get; set; }
        public DateTime TimestampDateTimeUtc { get => timestamp ?? DateTime.UtcNow.TruncateMillis(); set => timestamp = value; }
    }
}
