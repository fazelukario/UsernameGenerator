using System;
using System.IO;
using System.Linq;

namespace UsernameGenerator
{
    public static class UsernameGenerator
    {
        public static string[] Adjectives;
        public static string[] Animals;

        private static readonly FileSystemWatcher _fileSystemWatcher;
        private static readonly Random _random = new Random();

        static UsernameGenerator()
        {
            string resourcesPath = Directory.GetDirectories(Directory.GetCurrentDirectory(), "UsernameGenerator", SearchOption.AllDirectories).FirstOrDefault();

            if (resourcesPath != null && Directory.Exists(resourcesPath))
            {
                LoadResources(resourcesPath);

                _fileSystemWatcher = new FileSystemWatcher(resourcesPath, "*.txt")
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

                _fileSystemWatcher.Changed += ReloadResources;
                _fileSystemWatcher.Created += ReloadResources;
                _fileSystemWatcher.Deleted += ReloadResources;
                _fileSystemWatcher.Renamed += ReloadResources;

                _fileSystemWatcher.EnableRaisingEvents = true;
            }
            else
            {
                LoadResources();
            }
        }

        public static string GenerateUsername(int numberOfAdjectivesToUse = 1, int numberOfAnimalsToUse = 1,
                                              string separator = "", bool toLower = false, bool fancyUsernames = false,
                                              int? maxLength = null, bool addNumbers = false, int minNumbersValue = 1,
                                              int maxNumbersValue = 1000)
        {
            string username = string.Empty;

            for (int i = 0; i < numberOfAdjectivesToUse; i++)
                username += Adjectives[_random.Next(Adjectives.Length)];

            username += separator;

            for (int i = 0; i < numberOfAnimalsToUse; i++)
                username += Animals[_random.Next(Animals.Length)];

            if (maxLength != null && username.Length > maxLength)
                username = username.Substring(0, maxLength.Value);

            if (fancyUsernames)
                username = ToFancyUsername(username);

            if (toLower)
                username = username.ToLower();

            if (addNumbers)
                username += _random.Next(minNumbersValue, maxNumbersValue);

            return username;
        }

        public static string ToFancyUsername(string username)
        {
            if (_random.Next(0, 100) < 50)
                username = username.Replace("o", "0");

            if (_random.Next(0, 100) < 50)
                username = username.Replace("l", "1");

            if (_random.Next(0, 100) < 50)
                username = username.Replace("e", "3");

            return username;
        }

        private static void LoadResources(string resourcesPath = null)
        {
            if (resourcesPath == null)
            {
                Adjectives = Resources.Adjectives;
                Animals = Resources.Animals;
                return;
            }

            if (File.Exists(Path.Combine(resourcesPath, "Adjectives.txt")))
                Adjectives = File.ReadAllLines(Path.Combine(resourcesPath, "Adjectives.txt"));
            else
                Adjectives = Resources.Adjectives;

            if (File.Exists(Path.Combine(resourcesPath, "Animals.txt")))
                Animals = File.ReadAllLines(Path.Combine(resourcesPath, "Animals.txt"));
            else
                Animals = Resources.Animals;
        }

        private static void ReloadResources(object sender, FileSystemEventArgs e)
        {
            switch (e.Name)
            {
                case "Adjectives.txt":
                    if (File.Exists(e.FullPath))
                        Adjectives = File.ReadAllLines(e.FullPath);
                    else
                        Adjectives = Resources.Adjectives;
                    break;
                case "Animals.txt":
                    if (File.Exists(e.FullPath))
                        Animals = File.ReadAllLines(e.FullPath);
                    else
                        Animals = Resources.Animals;
                    break;
            }
        }
    }
}