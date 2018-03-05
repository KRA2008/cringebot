using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class GraphViewModel : FreshBasePageModel
    {
        private IEnumerable<Memory> _memories;

        public string Title { get; set; }
        public Command ViewStatsCommand { get; set; }

        public GraphViewModel()
        {
            ViewStatsCommand = new Command(async () =>
            {
                await ViewStats();
            });
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            _memories = (IEnumerable<Memory>) initData;
            Title = "Graph";
            if (_memories.Count() == 1)
            {
                Title += " (" + _memories.First().Description + ")";
            }
        }

        public async Task ViewStats()
        {
            await CoreMethods.PushPageModel<StatsViewModel>(_memories);
        }
    }
}