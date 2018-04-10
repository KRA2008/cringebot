using System;
using System.IO;

namespace Cringebot.Wrappers
{
    public interface IFileImportStore
    {
        void WriteFile(string data);
        string ReadFile();
    }

    public class FileImportStore : IFileImportStore
    {
        private const string MEMORY_IMPORT_FILENAME = "memoryImport";

        public void WriteFile(string data)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(appData, MEMORY_IMPORT_FILENAME);
            File.WriteAllText(filePath, data);
        }

        public string ReadFile()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(appData, MEMORY_IMPORT_FILENAME);
            return File.Exists(filePath) ? File.ReadAllText(filePath) : null;
        }
    }
}