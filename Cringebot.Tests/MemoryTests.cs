﻿using Cringebot.Model;
using NUnit.Framework;
using SharpTestsEx;
using System;
using System.Linq;

namespace Cringebot.Tests
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
