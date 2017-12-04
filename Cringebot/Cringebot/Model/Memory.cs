using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cringebot.Model
{
    public class Memory : INotifyPropertyChanged
    {
        public string Description { get; set; }
        public List<DateTime> Occurrences { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
