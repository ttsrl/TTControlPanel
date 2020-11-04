using System;
using System.Collections.Generic;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexOrderGetModel
    {
        public List<Order> Orders { get; set; }
        public string OrderBy { get; set; }
    }
}
