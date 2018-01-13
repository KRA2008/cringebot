using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cringebot.Model
{
    public class Memory : INotifyPropertyChanged
    {
        public string Description { get; set; }
        public ObservableCollection<DateTime> Occurrences { get; }

        public Memory()
        {
            Occurrences = new ObservableCollection<DateTime>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
