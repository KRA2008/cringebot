using Cringebot.Model;
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

        public DetailsPageModel(IAppDataStore dataStore)
        {
            _dataStore = dataStore;

            DeleteOccurrenceCommand = new Command((dateTime) =>
            {
                Memory.Occurrences.Remove((DateTime)dateTime);
            });
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            Memory = (Memory)initData;
        }
    }
}
