using Cringebot.Model;
using Cringebot.Utilities;
using Cringebot.Wrappers;
using FreshMvvm;
using System;
using Xamarin.Forms;

namespace Cringebot.PageModel
{
    public class DetailsPageModel : FreshBasePageModel
    {
        public Memory Memory { get; set; }

        public Command DeleteOccurrenceCommand { get; }

        private IAppDataStore _dataStore;
        private IInStorageMemoryUpdater _updater;

        public DetailsPageModel(IAppDataStore dataStore, IInStorageMemoryUpdater updater)
        {
            _dataStore = dataStore;
            _updater = updater;

            DeleteOccurrenceCommand = new Command((dateTime) =>
            {
                Memory.Occurrences.Remove((DateTime)dateTime);
            });

            PropertyChanged += (sender, args) => 
            {
                if(args.PropertyName == nameof(Memory))
                {
                    Memory.PropertyChanged += (inSender, inArgs) =>
                    {
                        _updater.UpdateMemory(Memory);
                    };
                    Memory.Occurrences.CollectionChanged += (inSender, inArgs) =>
                    {
                        _updater.UpdateMemory(Memory);
                    };
                }
            };
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            Memory = (Memory)initData;
        }
    }
}
