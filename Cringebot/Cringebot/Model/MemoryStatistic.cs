using PropertyChanged;

namespace Cringebot.Model
{
    [AddINotifyPropertyChangedInterface]
    public class MemoryStatistic
    {
        public string Description { get; set; }
        public double Number { get; set; }
    }
}