using System;
using System.IO;
using System.Linq;
using InfinniPlatform.Sdk.Hosting;

namespace InfinniPlatform.Watcher
{
    public class Watcher : ApplicationEventHandler
    {
        public Watcher(WatcherSettings settings) : base(1)
        {
            _settings = settings;
        }

        private readonly WatcherSettings _settings;

        public override void OnAfterStart()
        {
            Extensions.CheckSettings(_settings);

            Console.WriteLine("HALT! You are now watched by InfinniPlatform.Watch!");

            SyncDirectoriesIfNeeded();

            var watcher = new FileSystemWatcher
                          {
                              Path = _settings.SourceDirectory,
                              IncludeSubdirectories = true
                          };

            watcher.Changed += (sender, eventArgs) => Sync(eventArgs);
            watcher.Created += (sender, eventArgs) => Create(eventArgs);
            watcher.Deleted += (sender, eventArgs) => Delete(eventArgs);
            watcher.Renamed += (sender, eventArgs) => Sync(eventArgs);

            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Changes within directory {_settings.SourceDirectory} will now be transferred to directory {_settings.DestinationDirectory}.");
        }

        private void SyncDirectoriesIfNeeded()
        {
            var sourceFiles = Directory.GetFiles(_settings.SourceDirectory, "*.*", SearchOption.AllDirectories);
            var destFiles = Directory.GetFiles(_settings.DestinationDirectory, "*.*", SearchOption.AllDirectories);

            if (destFiles.Length != sourceFiles.Length)
            {
                Console.WriteLine($"Directories {_settings.SourceDirectory} {_settings.DestinationDirectory} are not same. Syncing...");

                Directory.Delete(_settings.DestinationDirectory, true);

                foreach (var dirPath in Directory.GetDirectories(_settings.SourceDirectory, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(_settings.SourceDirectory, _settings.DestinationDirectory));

                foreach (var newPath in Directory.GetFiles(_settings.SourceDirectory, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(_settings.SourceDirectory, _settings.DestinationDirectory), true);

                Extensions.ConsoleLogSync();
            }
        }

        private void Delete(FileSystemEventArgs eventArgs)
        {
            var extension = Path.GetExtension(eventArgs.FullPath);

            if (extension != string.Empty)
            {
                if (_settings.WatchingFileExtensions.Contains(extension))
                {
                    var part = eventArgs.FullPath.ToPartPath(_settings.SourceDirectory);

                    Extensions.ConsoleLogEvent(eventArgs, part);

                    Extensions.TryExecute(() =>
                    {
                        File.Delete(Path.Combine(_settings.DestinationDirectory, part));
                        Extensions.ConsoleLogSync();
                    });
                }
            }
            else
            {
                var part = eventArgs.FullPath.ToPartPath(_settings.SourceDirectory);

                Extensions.ConsoleLogEvent(eventArgs, part);

                Extensions.TryExecute(() =>
                {
                    Directory.Delete(Path.Combine(_settings.DestinationDirectory, part), true);
                    Extensions.ConsoleLogSync();
                });
            }
        }

        private void Create(FileSystemEventArgs eventArgs)
        {
            var extension = Path.GetExtension(eventArgs.FullPath);

            if (extension != string.Empty)
            {
                if (_settings.WatchingFileExtensions.Contains(extension))
                {
                    var part = eventArgs.FullPath.ToPartPath(_settings.SourceDirectory);

                    Extensions.ConsoleLogEvent(eventArgs, part);

                    Extensions.TryExecute(() => { File.Copy(eventArgs.FullPath, Path.Combine(_settings.DestinationDirectory, part), true); });

                    Extensions.ConsoleLogSync();
                }
            }
            else
            {
                var part = eventArgs.FullPath.ToPartPath(_settings.SourceDirectory);

                Extensions.ConsoleLogEvent(eventArgs, part);

                Extensions.TryExecute(() => { Directory.CreateDirectory(Path.Combine(_settings.DestinationDirectory, part)); });

                Extensions.ConsoleLogSync();
            }
        }

        private void Sync(FileSystemEventArgs eventArgs)
        {
            var extension = Path.GetExtension(eventArgs.FullPath);

            if (extension != string.Empty)
            {
                if (_settings.WatchingFileExtensions.Contains(extension))
                {
                    var part = eventArgs.FullPath.ToPartPath(_settings.SourceDirectory);

                    Extensions.ConsoleLogEvent(eventArgs, part);

                    Extensions.TryExecute(() => { File.Copy(eventArgs.FullPath, Path.Combine(_settings.DestinationDirectory, part), true); });

                    Extensions.ConsoleLogSync();
                }
            }
        }
    }
}