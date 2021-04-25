using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexUserModel
    {
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class NewUserGetModel
    {
        public List<Role> Roles { get; set; }
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
        public string Role { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfPassword { get; set; }
    }

    public class EditUserGetModel
    {
        public List<Role> Roles { get; set; }
        public User User { get; set; }
        public int Error { get; set; }
    }

    public class EditUserPostModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        public string Password { get; set; }
        public string ConfPassword { get; set; }
    }

    public class DetailsUserGetModel
    {
        public User User { get; set; }
        public List<ApplicationVersion> ApplicationVersions { get; set; }
        public List<ProductKey> ProductKeys { get; set; }
        public List<HID> Hids { get; set; }
    }
}
