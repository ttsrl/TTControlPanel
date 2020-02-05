﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexApplicationModel
    {
        public List<Application> Applications { get; set; }
    }

    public class NewApplicationPostModel
    {
        public bool AutomaticCode { get; set; }
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Release { get; set; }
        [Required]
        public string Major { get; set; }
        [Required]
        public string Minor { get; set; }
    }

    public class NewApplicationGetModel
    {
        public int Error { get; set; }
    }

    public class VersionsApplicationGetModel
    {
        public Application Application { get; set; }
    }
}
