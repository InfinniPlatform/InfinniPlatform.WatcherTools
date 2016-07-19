using System.IO;

namespace InfinniPlatform.Watcher
{
    public class WatcherSettings
    {
        /// <summary>
        ///     Настройки хранения метаданных.
        /// </summary>
        public const string SectionName = "watcher";

        public WatcherSettings()
        {
            _sourceDirectory = string.Empty;
            _destinationDirectory = string.Empty;
            WatchingFileExtensions = new[] {".json"};
        }

        private string _sourceDirectory;
        private string _destinationDirectory;

        /// <summary>
        ///     Источник метаданных.
        /// </summary>
        public string SourceDirectory
        {
            get
            {
                return string.IsNullOrEmpty(_sourceDirectory)
                           ? _sourceDirectory
                           : new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), _sourceDirectory)).FullName;
            }
            set { _sourceDirectory = value; }
        }

        /// <summary>
        ///     Синхронизируемая папка.
        /// </summary>
        public string DestinationDirectory
        {
            get
            {
                return string.IsNullOrEmpty(_destinationDirectory)
                           ? _sourceDirectory
                           : new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), _destinationDirectory)).FullName;
            }
            set { _destinationDirectory = value; }
        }

        /// <summary>
        ///     Расширения синхронизируемых файлов.
        /// </summary>
        public string[] WatchingFileExtensions { get; set; }
    }
}