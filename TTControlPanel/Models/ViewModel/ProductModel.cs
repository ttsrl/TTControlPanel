using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexProductModel
    {
        public int Error { get; set; }
        public List<Product> Products { get; set; }
        public string OrderBy { get; set; }
    }

    public class NewProductGetModel
    {
        public int Error { get; set; }
    }

    public class NewProductPostModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string VAT { get; set; }
        [Required]
        public string SellingPrice { get; set; }
        [Required]
        public string AveragePrice { get; set; }
    }

    public class EditProductGetModel
    {
        public Product Product { get; set; }
        public int Error { get; set; }
    }

    public class EditProductPostModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string VAT { get; set; }
        [Required]
        public string SellingPrice { get; set; }
        [Required]
        public string AveragePrice { get; set; }
    }
}