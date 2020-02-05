using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTControlPanel.Models
{
    public class User
    {
        private string _username = "";
        private DateTime? _registrationDate;

        public int Id { get; set; }
        public string Barcode { get; set; }
        //[ForeignKey("UserDataId")] 
        //public UserData UserData { get; set; }
        public bool? Visible { get; set; }
        public string Username
        {
            get => _username;
            set => _username = value.ToLower().Replace(" ", "").Replace("'", "");
        }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [ForeignKey("RoleId")] 
        public Role Role { get; set; }
        public DateTime RegistrationDate
        {
            get => _registrationDate ?? DateTime.Now;
            set => _registrationDate = value;
        }
    }
}
