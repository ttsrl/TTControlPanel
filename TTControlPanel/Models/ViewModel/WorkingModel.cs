using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexWorkingGetModel
    {
        public List<Working> Workings { get; set; }
        public List<Order> Orders { get; set; }
        public int Error { get; set; }
        public string OrderBy { get; set; }
    }

    public class NewWorkingGetModel
    {
        public Order Selected { get; set; }
        public List<Order> Orders { get; set; }
        public List<Client> Clients { get; set; }
        public int Error { get; set; }
    }

    public class NewWorkingPostModel
    {
        [Required]
        public int Client { get; set; }
        [Required]
        public int Order { get; set; }
    }
}
