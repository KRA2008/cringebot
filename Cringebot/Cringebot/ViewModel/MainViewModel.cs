using Cringebot.Model;
using FreshMvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cringebot.ViewModel
{
    public class MainViewModel : FreshBasePageModel
    {
        public ObservableCollection<Memory> Memories { get; set; }

        public MainViewModel()
        {
            var tempMemories = new List<Memory>();

            for(var i = 0; i<100; i++)
            {
                tempMemories.Add(new Memory
                {
                    Description = "Memory" + i
                });
            }

            Memories = new ObservableCollection<Memory>(tempMemories);
        }
    }
}