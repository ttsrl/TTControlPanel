using System;

namespace TTControlPanel.Models
{
    public class License
    {

        private DateTime? releaseDate;
        private string confirmCode;

        public int Id { get; set; }
        public ProductKey ProductKey { get; set; }
        public ApplicationVersion ApplicationVersion { get; set; }
        public HID Hid { get; set; }
        public string ConfirmCode { get => confirmCode; set => confirmCode = value.ToUpper(); }
        public bool Active { get; set; }
        public bool Banned { get; set; }
        public DateTime? ActivateDateTime { get; set; }
        public Client Client { get; set; }
        public string Notes { get; set; }
        public DateTime ReleaseDate
        {
            get => releaseDate ?? DateTime.Now;
            set => releaseDate = value;
        }
    }
}
