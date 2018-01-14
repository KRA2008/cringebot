using Xamarin.Forms;

namespace Cringebot.Page.CustomElements
{
    public class DeselectListView : ListView
    {
        public DeselectListView()
        {
            ItemTapped += (args, sender) =>
            {
                SelectedItem = null;
            };
        }
    }
}
