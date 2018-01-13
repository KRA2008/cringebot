using Cringebot.Model;
using FreshMvvm;

namespace Cringebot.PageModel
{
    public class DetailsPageModel : FreshBasePageModel
    {
        public Memory Memory { get; set; }

        public override void Init(object initData)
        {
            base.Init(initData);
            Memory = (Memory)initData;
        }
    }
}
