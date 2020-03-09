using System;

namespace TTControlPanel.Models
{
    public class ProductKey
    {
        public enum PKType { Normal, Time, Trial }
        private DateTime? generateDate;

        public int Id { get; set; }
        public string Key { get; set; }
        public User GenerateUser { get; set; }
        public DateTime GenerateDateTime
        {
            get => generateDate ?? DateTime.Now;
            set => generateDate = value;
        }
        public PKType Type { get; set; }

    }
}
