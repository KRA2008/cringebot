using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cringebot.PageModel
{
    public class DetailsPageModel : FreshBasePageModel
    {
        public Memory Memory { get; set; }

        public Command DeleteOccurrenceCommand { get; }
        public Command DeleteMemoryCommand { get; }

        private IAppDataStore _dataStore;

        public DetailsPageModel(IAppDataStore dataStore)
        {
            _dataStore = dataStore;

            DeleteOccurrenceCommand = new Command((dateTime) =>
            {
                Memory.Occurrences.Remove((DateTime)dateTime);
            });

            DeleteMemoryCommand = new Command(async () => //argh! get a decent async commmand
            {
                await DeleteMemory(Memory);
            });
        }

        public async Task DeleteMemory(Memory memory)
        {
            await CoreMethods.PopPageModel(memory);
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            Memory = (Memory)initData;
        }
    }
}
