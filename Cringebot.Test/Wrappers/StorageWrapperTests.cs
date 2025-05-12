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
            PersistentStorage.MEMORY_LIST_STORE_KEY.Should().Be.EqualTo("memoryList");
            PersistentStorage.LIMIT_LIST_STORE_KEY.Should().Be.EqualTo("limitList");
            PersistentStorage.SIMULATE_STORE_KEY.Should().Be.EqualTo("simulate");
        }
    }
}
