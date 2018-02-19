using System;
using System.Collections.ObjectModel;
using PropertyChanged;

namespace Cringebot.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Memory
    {
        public string Description { get; set; }
        public ObservableCollection<DateTime> Occurrences { get; }

        public Memory()
        {
            Occurrences = new ObservableCollection<DateTime>();
        }
    }
}