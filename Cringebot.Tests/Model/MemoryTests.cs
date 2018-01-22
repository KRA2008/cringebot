using Cringebot.Model;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.Model
{
    public class MemoryTests
    {
        private Memory _memory;

        [SetUp]
        public void Setup()
        {
            _memory = new Memory();
        }

        public class Ctor : MemoryTests
        {
            [Test]
            public void ShouldInitializeOccurencesList()
            {
                _memory.Occurrences.Should().Not.Be.Null();
            }
        }
    }
}
