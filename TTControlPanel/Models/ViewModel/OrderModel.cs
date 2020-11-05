using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexOrderGetModel
    {
        public List<Order> Orders { get; set; }
        public List<Working> Workings { get; set; }
        public string OrderBy { get; set; }
    }

    public class DetailsOrderGetModel
    {
        public Order Order { get; set; }
        public Working Working { get; set; }
    }

    public class NewOrderGetModel
    {
        public int Error { get; set; }
        public List<Client> Clients { get; set; }
        public int Number { get; set; }
        public List<Invoice> Invoices { get; set; }
    }

    public class NewOrderPostModel
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public int Client { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Invoice { get; set; }
        [Required]
        public DateTime DeliveryDate { get; set; }
    }

    public class EditOrderGetModel
    {
        public int Error { get; set; }
        public List<Client> Clients { get; set; }
        public List<Invoice> Invoices { get; set; }
        public Order Order { get; set; }
    }

    public class EditOrderPostModel
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public int Client { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Invoice { get; set; }
        [Required]
        public DateTime DeliveryDate { get; set; }
    }
}
