using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace Cringebot.Model
{
    public class Memory : INotifyPropertyChanged
    {
        public string Description { get; set; }
        public ObservableCollection<DateTime> Occurrences { get; }
        public Command AddOccurrenceCommand { get; } //TODO: should this viewy thing be allowed here?....

        public Memory()
        {
            Occurrences = new ObservableCollection<DateTime>();

            AddOccurrenceCommand = new Command(() =>
            {
                Occurrences.Add(DateTime.Now);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
