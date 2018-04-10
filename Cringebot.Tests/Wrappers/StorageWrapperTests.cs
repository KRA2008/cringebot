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
            PropertiesWrapper.MEMORY_LIST_STORE_KEY.Should().Be.EqualTo("memoryList");
            PropertiesWrapper.LIMIT_LIST_STORE_KEY.Should().Be.EqualTo("limitList");
            PropertiesWrapper.SIMULATE_STORE_KEY.Should().Be.EqualTo("simulate");
        }
    }
}
