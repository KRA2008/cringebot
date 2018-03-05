using Cringebot.Model;
using FreshMvvm;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class DetailsViewModel : FreshBasePageModel
    {
        public Memory Memory { get; set; }

        public Command DeleteOccurrenceCommand { get; }
        public Command DeleteMemoryCommand { get; }
        public Command ViewGraphCommand { get; }

        public DetailsViewModel()
        {
            DeleteOccurrenceCommand = new Command(dateTime =>
            {
                Memory.Occurrences.Remove((DateTime)dateTime);
            });

            DeleteMemoryCommand = new Command(async () => //argh! get a decent async commmand
            {
                await DeleteMemory(Memory);
            });

            ViewGraphCommand = new Command(async () =>
            {
                await ViewGraph(Memory);
            });
        }

        public async Task DeleteMemory(Memory memory)
        {
            var delete = await CoreMethods.DisplayAlert("Confirm", "Really delete?", "Delete", "Cancel");
            if (delete)
            {
                await CoreMethods.PopPageModel(memory);
            }
        }

        public async Task ViewGraph(Memory memory)
        {
            await CoreMethods.PushPageModel<GraphViewModel>(new[] {memory});
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            Memory = (Memory)initData;
        }
    }
}
