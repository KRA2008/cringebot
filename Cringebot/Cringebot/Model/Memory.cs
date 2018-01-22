using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cringebot.Model
{
    public class Memory : INotifyPropertyChanged
    {
        private string _id;
        public string Id
        {
            get => _id ?? (_id = Guid.NewGuid().ToString());
            set
            {
                if (_id == null)
                {
                    _id = value;
                }
            }
        }
        public string Description { get; set; }
        public ObservableCollection<DateTime> Occurrences { get; }

        public Memory()
        {
            Occurrences = new ObservableCollection<DateTime>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
