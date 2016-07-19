using System;
using System.IO;

namespace InfinniPlatform.Watcher
{
    public static class Extensions
    {
        public static string ToPartPath(this string s, string sourcePath)
        {
            return s.Replace(sourcePath, string.Empty).TrimStart(Path.DirectorySeparatorChar);
        }

        public static bool CheckSettings(WatcherSettings settings)
        {
            var isCorrectSettings = true;

            var n = Environment.NewLine;

            var s = $"{n}Add watcher setting to AppExtentions.json configuration file:" +
                    $"{n}  /* Настройки наблюдателя */" +
                    $"{n}  \"watcher\": {{" +
                    $"{n}      /* Директория источника метаданных */" +
                    $"{n}      \"SourceDirectory\": <path>," +
                    $"{n}      /* Директория для синхнонизации */" +
                    $"{n}      \"DestinationDirectory\": <path>," +
                    $"{n}      /* Расширения синхронизируемых файлов */" +
                    $"{n}      \"WatchingFileExtensions\": [" +
                    $"{n}          \".json\"" +
                    $"{n}      ]" +
                    $"{n}  }}";

            if (string.IsNullOrEmpty(settings.SourceDirectory))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("SourceDictionary in watcher settings cannot be empty.");
                Console.BackgroundColor = ConsoleColor.Black;
                isCorrectSettings = false;
            }

            if (string.IsNullOrEmpty(settings.DestinationDirectory))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("DestinationDirectory in watcher settings cannot be empty.");
                Console.BackgroundColor = ConsoleColor.Black;
                isCorrectSettings = false;
            }

            if (!isCorrectSettings)
            {
                Console.WriteLine(s);
            }

            return isCorrectSettings;
        }

        public static void ConsoleLogEvent(FileSystemEventArgs eventArgs, string part)
        {
            Console.WriteLine($"{Environment.NewLine}{DateTime.Now.ToString("G")}{Environment.NewLine}File {part} was {eventArgs.ChangeType}.");
        }

        public static void ConsoleLogSync()
        {
            Console.WriteLine("Sync complete.");
        }

        public static void TryExecute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Error.");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(e.ToString());
            }
        }
    }
}