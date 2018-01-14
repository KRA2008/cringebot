using Cringebot.Wrappers;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.Wrappers
{
    public class StorageWrapperTests
    {
        [Test]
        public void ShouldExposeConstants()
        {
            StorageWrapper.MEMORY_LIST_STORE_KEY.Should().Be.EqualTo("memoryList");
            StorageWrapper.SHOW_LIST_STORE_KEY.Should().Be.EqualTo("showList");
            StorageWrapper.SIMULATE_STORE_KEY.Should().Be.EqualTo("simulate");
        }
    }
}
