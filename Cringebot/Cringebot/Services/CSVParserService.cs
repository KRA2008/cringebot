using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cringebot.Model;

namespace Cringebot.Services
{
    public interface ICsvParserService
    {
        IEnumerable<string> ParseLines(string text);
        Memory ParseMemory(string line);
        string StringifyMemories(IEnumerable<Memory> memories);
    }

    public class CsvParserService : ICsvParserService
    {
        private const char LINE_ENDER = '\n';

        public IEnumerable<string> ParseLines(string text)
        {
            return text.Split(LINE_ENDER);
        }

        public Memory ParseMemory(string line)
        {
            var pieces = line.Split(',');
            var pieceExists = pieces.Length > 1;
            bool parseWorked;
            var parsed = new DateTime();
            parseWorked = pieceExists && DateTime.TryParse(pieces[1], out parsed);

            return new Memory
            {
                Description = pieces[0],
                Occurrences = parseWorked ? new ObservableCollection<DateTime>(new List<DateTime>
                {
                    parsed
                }) : new ObservableCollection<DateTime>()
            };
        }

        public string StringifyMemories(IEnumerable<Memory> memories)
        {
            var output = "";
            foreach (var memory in memories)
            {
                if (memory.Occurrences.Any())
                {
                    foreach (var occurrence in memory.Occurrences)
                    {
                        output += memory.Description;
                        output += ",";
                        output += occurrence.ToString("O");
                        output += LINE_ENDER;
                    }
                }
                else
                {
                    output += memory.Description;
                    output += LINE_ENDER;
                }
            }

            var lastIndexOfEnder = output.LastIndexOf(LINE_ENDER);
            return output.Substring(0, lastIndexOfEnder);
        }

    }
}