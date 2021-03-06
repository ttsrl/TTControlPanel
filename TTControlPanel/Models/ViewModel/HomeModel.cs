﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTControlPanel.Models.ViewModel
{
    public class HomeGetModel
    {
        public List<GitCommit> Commits { get; set; }
    }

    public class GitCommit
    {
        public string AuthorName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Message { get; set; }
        public int Files { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
    }

    public class GroupGitCommit
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
