using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cringebot.Model;
using Cringebot.Services;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.Services
{
    public class CsvParserServiceTests
    {
        private CsvParserService _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new CsvParserService();
        }

        public class ParseLinesMethod : CsvParserServiceTests
        {
            [Test]
            public void ShouldSeparateByLineSeparator()
            {
                //act
                var lines = _parser.ParseLines("one\ntwo\nthree");

                //assert
                lines.Should().Have.SameSequenceAs("one", "two", "three");
            }
        }

        public class ParseMemory : CsvParserServiceTests
        {
            [Test]
            public void ShouldParseMemory()
            {
                //arrange
                const string MEMORY_DESCRIPTION = "that one time";
                var memoryOccurrence = new DateTime(1, 2, 3, 4, 5, 6);

                //act
                var memory = _parser.ParseMemory(MEMORY_DESCRIPTION + "," + memoryOccurrence.ToString("O"));

                //assert
                memory.Satisfies(m =>
                    m.Description == MEMORY_DESCRIPTION &&
                    m.Occurrences.First() == memoryOccurrence);
            }
        }

        public class StringifyMemories : CsvParserServiceTests
        {
            [Test]
            public void ShouldStringifyMemories()
            {
                //arrange
                const string MEM_1_DESC = "thing 1";
                const string MEM_2_DESC = "thing 2";
                const string MEM_3_DESC = "thing 3";
                var mem1Occ1 = new DateTime(1,1,1,1,1,1);
                var mem1Occ2 = new DateTime(2,2,2,2,2,2);
                var mem2Occ1 = new DateTime(3,3,3,3,3,3);
                var mem2Occ2 = new DateTime(4,4,4,4,4,4);
                var mem1 = new Memory
                {
                    Description = MEM_1_DESC,
                    Occurrences = new ObservableCollection<DateTime>(new List<DateTime>
                    {
                        mem1Occ1,
                        mem1Occ2
                    })
                };
                var mem2 = new Memory
                {
                    Description = MEM_2_DESC,
                    Occurrences = new ObservableCollection<DateTime>(new List<DateTime>
                    {
                        mem2Occ1,
                        mem2Occ2
                    })
                };
                var mem3 = new Memory
                {
                    Description = MEM_3_DESC
                };

                //act
                var thing = _parser.StringifyMemories(new[] {mem1, mem3, mem2});

                //assert
                thing.Should().Be.EqualTo(
                    MEM_1_DESC + "," + mem1Occ1.ToString("O") + '\n' +
                    MEM_1_DESC + "," + mem1Occ2.ToString("O") + '\n' +
                    MEM_3_DESC + '\n' +
                    MEM_2_DESC + "," + mem2Occ1.ToString("O") + '\n' +
                    MEM_2_DESC + "," + mem2Occ2.ToString("O")
                );
            }
        }
    }
}