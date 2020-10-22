using System;
using System.Collections.Generic;

namespace TTControlPanel.Models.ViewModel
{
    public class IndexAutomationModel
    {
        public AutomationItem Automation1 { get; set; }
    }

    public class AutomationItem
    {
        public AutomationState State { get; set; }
        public bool RelayValue { get; set; }
        public string StringValue { get; set; }
        public int IntValue { get; set; }
    }

    public enum AutomationState
    {
        Offline = 0,
        Online = 1
    }
}
