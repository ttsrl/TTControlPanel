using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models
{
    public class ProductKey
    {
        private DateTime? generateDate;

        public int Id { get; set; }
        public string Key { get; set; }
        public User GenerateUser { get; set; }
        public DateTime GenerateDateTime
        {
            get => generateDate ?? DateTime.Now;
            set => generateDate = value;
        }

    }
}
