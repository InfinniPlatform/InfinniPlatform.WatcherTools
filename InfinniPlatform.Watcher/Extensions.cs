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

        public static void CheckSettings(WatcherSettings settings)
        {
            if (string.IsNullOrEmpty(settings.SourceDirectory))
            {
                throw new ArgumentException("SourceDictionary in watcher settings cannot be empty.");
            }

            if (string.IsNullOrEmpty(settings.DestinationDirectory))
            {
                throw new ArgumentException("DestinationDirectory in watcher settings cannot be empty.");
            }
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
                Console.WriteLine("Got error!");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(e.ToString());
            }
        }
    }
}