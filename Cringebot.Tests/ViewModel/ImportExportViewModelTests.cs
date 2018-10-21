using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.Services;
using Cringebot.ViewModel;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class ImportExportViewModelTests
    {
        private ImportExportViewModel _viewModel;
        private Mock<ICsvParserService> _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new Mock<ICsvParserService>();
            _viewModel = new ImportExportViewModel(_parser.Object);
        }

        public class ExportCommandProperty : ImportExportViewModelTests
        {
            [Test]
            public void ShouldFillExportTextWithMemories()
            {
                //arrange
                var memories = new List<Memory>();
                const string STRINGIFIED = "hello there";
                _parser.Setup(p => p.StringifyMemories(memories)).Returns(STRINGIFIED);

                //act
                _viewModel.Init(memories);
                _viewModel.ExportCommand.Execute(null);

                //assert
                _viewModel.ImportExportText.Should().Be.EqualTo(STRINGIFIED);
            }
        }

        public class ImportAsyncMethod : ImportExportViewModelTests
        {
            [Test]
            public async Task ShouldParseMemoriesAndAddAndPopBack()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;

                var existingMemory = new Memory
                {
                    Description = "whatever"
                };

                const string INPUT_FULL_STRING = "blah blah blah";
                const string INPUT_PARTIAL_STRING_1 = "hello";
                const string INPUT_PARTIAL_STRING_2 = "hi";
                const string INPUT_PARTIAL_STRING_3 = "greeting";
                const string INPUT_PARTIAL_STRING_4 = "never";

                const string DUPLICATE_MEMORY_DESC = "this duplicate";
                var partialStrings = new[]
                {
                    INPUT_PARTIAL_STRING_1,
                    INPUT_PARTIAL_STRING_2,
                    INPUT_PARTIAL_STRING_3,
                    INPUT_PARTIAL_STRING_4
                };
                var mem1 = new Memory
                {
                    Description = "haha"
                };
                var mem2 = new Memory
                {
                    Description = DUPLICATE_MEMORY_DESC,
                    Occurrences = new ObservableCollection<DateTime>(new List<DateTime>
                    {
                        new DateTime()
                    })
                };
                var mem3 = new Memory
                {
                    Description = "ok"
                };
                var mem4 = new Memory
                {
                    Description = DUPLICATE_MEMORY_DESC,
                    Occurrences = new ObservableCollection<DateTime>(new List<DateTime>
                    {
                        new DateTime()
                    })
                };
                _parser.Setup(p => p.ParseLines(INPUT_FULL_STRING)).Returns(partialStrings);
                _parser.Setup(p => p.ParseMemory(INPUT_PARTIAL_STRING_1)).Returns(mem1);
                _parser.Setup(p => p.ParseMemory(INPUT_PARTIAL_STRING_2)).Returns(mem2);
                _parser.Setup(p => p.ParseMemory(INPUT_PARTIAL_STRING_3)).Returns(mem3);
                _parser.Setup(p => p.ParseMemory(INPUT_PARTIAL_STRING_4)).Returns(mem4);

                _viewModel.ImportExportText = INPUT_FULL_STRING;

                //act
                _viewModel.Init(new List<Memory>
                {
                    existingMemory
                });
                await _viewModel.ImportAsync();

                //assert
                coreMethods.Verify(c =>
                    c.PopPageModel(
                        It.Is<SettingsPushPackage>(p =>
                            p.Memories.SequenceEqual(new[] {existingMemory, mem1, mem2, mem3})), false, true));
                mem2.Occurrences.Count.Should().Be.EqualTo(2);
            }
        }
    }
}