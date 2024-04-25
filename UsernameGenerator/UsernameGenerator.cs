using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace UsernameGenerator
{
    /// <summary>
    /// A utility class for generating usernames.
    /// </summary>
    public static class UsernameGenerator
    {
        /// <summary>
        /// The array of adjectives used for generating usernames.
        /// </summary>
        public static string[] Adjectives;

        /// <summary>
        /// The array of animals used for generating usernames.
        /// </summary>
        public static string[] Animals;

        private static readonly FileSystemWatcher _fileSystemWatcher;
        private static readonly Random _random = new Random();

        static UsernameGenerator()
        {
            string resourcesPath = Directory.GetDirectories(Directory.GetCurrentDirectory(), "UsernameGenerator", SearchOption.AllDirectories).FirstOrDefault();

            if (resourcesPath != null && Directory.Exists(resourcesPath))
            {
                Debug.WriteLine("Loading resources from: " + resourcesPath);

                LoadResources(resourcesPath);

                Debug.WriteLine("Resources loaded");

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
                Debug.WriteLine("Loading default resources");

                LoadResources();

                Debug.WriteLine("Default resources loaded");
            }
        }

        /// <summary>
        /// Generates a username based on the specified parameters.
        /// </summary>
        /// <param name="numberOfAdjectivesToUse">The number of adjectives to use in the username.</param>
        /// <param name="numberOfAnimalsToUse">The number of animals to use in the username.</param>
        /// <param name="separator">The separator to use between adjectives and animals.</param>
        /// <param name="toLower">Indicates whether to convert the username to lowercase.</param>
        /// <param name="fancyUsername">Indicates whether to convert the username to a fancy username.</param>
        /// <param name="maxLength">The maximum length of the username.</param>
        /// <param name="addNumbers">Indicates whether to add numbers to the username.</param>
        /// <param name="minNumbersValue">The minimum value for the added numbers.</param>
        /// <param name="maxNumbersValue">The maximum value for the added numbers.</param>
        /// <returns>The generated username.</returns>
        public static string GenerateUsername(int numberOfAdjectivesToUse = 1, int numberOfAnimalsToUse = 1,
                                              string separator = "", bool toLower = false, bool fancyUsername = false,
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

            if (fancyUsername)
                username = ToFancyUsername(username);

            if (toLower)
                username = username.ToLower();

            if (addNumbers)
                username += _random.Next(minNumbersValue, maxNumbersValue);

            return username;
        }

        /// <summary>
        /// Converts the username to a fancy username.
        /// </summary>
        /// <param name="username">The username to convert.</param>
        /// <param name="randomizeReplacment">Indicates whether to randomize the replacement of characters.</param>
        /// <param name="randomizeReplacementCount">Indicates whether to randomize the count of characters to replace.</param>
        /// <returns>The converted fancy username.</returns>
        public static string ToFancyUsername(string username, bool randomizeReplacment = true, bool randomizeReplacementCount = true)
        {
            for (int i = 0; i < 3; i++)
            {
                bool replace = !randomizeReplacment || _random.Next(0, 100) < 50;

                if (!replace)
                    continue;

                int charsToReplace = 0;

                switch (i)
                {
                    case 0:
                        charsToReplace = username.Count(c => c == 'o');

                        if (charsToReplace == 0)
                            break;

                        username = new Regex(Regex.Escape("o")).Replace(username, "0",
                                                                        randomizeReplacementCount ? _random.Next(1, charsToReplace + 1) : charsToReplace);
                        break;

                    case 1:
                        charsToReplace = username.Count(c => c == 'l');

                        if (charsToReplace == 0)
                            break;

                        username = new Regex(Regex.Escape("l")).Replace(username, "1",
                                                                        randomizeReplacementCount ? _random.Next(1, charsToReplace + 1) : charsToReplace);
                        break;

                    case 2:
                        charsToReplace = username.Count(c => c == 'e');

                        if (charsToReplace == 0)
                            break;

                        username = new Regex(Regex.Escape("e")).Replace(username, "3",
                                                                        randomizeReplacementCount ? _random.Next(1, charsToReplace + 1) : charsToReplace);
                        break;
                }
            }

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
            Debug.WriteLine("Reloading resources");
            Debug.WriteLine("Resource: " + e.Name);
            Debug.WriteLine("Path: " + e.FullPath);
            Debug.WriteLine("Event: " + e.ChangeType);

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

            Debug.WriteLine("Resources reloaded");
        }
    }
}