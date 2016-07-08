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
            SourceDirectory = string.Empty;
            DestinationDirectory = string.Empty;
            WatchingFileExtensions = new[] {".json"};
        }

        /// <summary>
        ///     Источник метаданных.
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>
        ///     Синхронизируемая папка.
        /// </summary>
        public string DestinationDirectory { get; set; }

        /// <summary>
        ///     Расширения синхронизируемых файлов.
        /// </summary>
        public string[] WatchingFileExtensions { get; set; }
    }
}