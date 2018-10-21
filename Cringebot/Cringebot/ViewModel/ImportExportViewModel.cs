using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.Services;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class ImportExportViewModel : FreshBasePageModel
    {
        private readonly ICsvParserService _parserService;
        public string ImportExportText { get; set; }

        public Command ImportCommand { get; set; }
        public Command ExportCommand { get; set; }

        private List<Memory> _existingMemories;

        public ImportExportViewModel(ICsvParserService parserService)
        {
            _parserService = parserService;

            ExportCommand = new Command(() =>
            {
                ImportExportText = parserService.StringifyMemories(_existingMemories);
            });

            ImportCommand = new Command(async () =>
            {
                await ImportAsync();
            });
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            _existingMemories = (List<Memory>) initData;
        }

        public async Task ImportAsync()
        {
            var lines = _parserService.ParseLines(ImportExportText);
            foreach (var line in lines)
            {
                var parsedMemory = _parserService.ParseMemory(line);
                var existingMatchingMemory = _existingMemories.FirstOrDefault(m => m.Description == parsedMemory.Description);
                if (existingMatchingMemory != null)
                {
                    if (parsedMemory.Occurrences.Any())
                    {
                        existingMatchingMemory.Occurrences.Add(parsedMemory.Occurrences.First());
                    }
                }
                else
                {
                    _existingMemories.Add(_parserService.ParseMemory(line));
                }
            }

            await CoreMethods.PopPageModel(new SettingsPushPackage
            {
                Memories = _existingMemories
            });
        }
    }
}