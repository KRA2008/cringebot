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
            StorageWrapper.LIMIT_LIST_STORE_KEY.Should().Be.EqualTo("limitList");
            StorageWrapper.SIMULATE_STORE_KEY.Should().Be.EqualTo("simulate");
        }
    }
}
