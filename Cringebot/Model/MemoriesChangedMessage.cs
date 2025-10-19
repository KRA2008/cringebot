using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Cringebot.Model
{
    public class MemoriesChangedMessage : ValueChangedMessage<IEnumerable<Memory>>
    {
        public MemoriesChangedMessage(IEnumerable<Memory> memories) : base(memories) {}
    }

    public class ThemeChangedMessage : ValueChangedMessage<string>
    {
        public ThemeChangedMessage(string themeName) : base(themeName) {}
    }

    public class SettingsChangedMessage : ValueChangedMessage<Settings>
    {
        public SettingsChangedMessage(Settings settings) : base(settings) {}
    }

    public class SomethingChangedMessage : ValueChangedMessage<object>
    {
        public SomethingChangedMessage(object arg) : base(arg) {}
    }
}
