using System;
using System.ComponentModel.DataAnnotations.Schema;
using TTControlPanel.Utilities;

namespace TTControlPanel.Models
{
    public class User
    {
        private string _username = "";
        private DateTime? _registrationDate;
        private string email = "";
        private string name = "";
        private string surname = "";

        public int Id { get; set; }
        public bool Visible { get; set; }
        public string Username
        {
            get => _username;
            set => _username = value.ToLower().Replace(" ", "").Replace("'", "");
        }
        public string Name { get => name; set => name = value.ToTitleCase(); }
        public string Surname { get => surname; set => surname = value.ToTitleCase(); }
        public string Password { get; set; }
        public string Email { get => email; set => email = value.ToLower(); }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        public DateTime RegistrationDate
        {
            get => _registrationDate ?? DateTimeCE.Now;
            set => _registrationDate = value;
        }
    }
}
