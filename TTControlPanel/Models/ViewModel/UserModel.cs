using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexUserModel
    {
        public List<User> Users { get; set; }
    }

    public class NewUserGetModel
    {
        public int Error { get; set; }
    }

    public class NewUserPostModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfPassword { get; set; }
    }
}
