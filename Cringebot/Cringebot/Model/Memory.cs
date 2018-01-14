using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cringebot.Model
{
    public class Memory : INotifyPropertyChanged
    {
        private string _id;
        public string ID
        {
            get
            {
                if (_id == null)
                {
                    _id = Guid.NewGuid().ToString();
                }
                return _id;
            }
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
