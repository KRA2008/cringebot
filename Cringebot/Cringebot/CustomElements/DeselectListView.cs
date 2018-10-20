using Xamarin.Forms;

namespace Cringebot.CustomElements
{
    public class DeselectListView : ListView
    {
        public DeselectListView(ListViewCachingStrategy strategy) : base(strategy)
        {
            ItemTapped += (args, sender) =>
            {
                SelectedItem = null;
            };
        }
    }
}
