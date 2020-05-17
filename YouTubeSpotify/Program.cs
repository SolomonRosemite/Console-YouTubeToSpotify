using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System;

namespace YouTubeSpotify
{
    class Program
    {
        private static string[] songs;
        private static string[] unsureSongs;

        private static string pathToPython = @"C:\Python38\python.exe";
        private static string playlistUrl = "";

        static void Main(string[] args)
        {
            File.Delete("data.json");

            string name = GetPlaylistname();

            CreatePythonConnection(pathToPython, playlistUrl);
            AssignArrays("data.json");

            File.Delete("data.json");

            Spotify.FinishPlaylist(name, songs);

            if (unsureSongs != null)
                Spotify.AddUnsureSongs(unsureSongs);

            if (Spotify.notFoundSongs.Count != 0)
            {
                Console.WriteLine("Songs that couldn't be added:\n");

                foreach (var item in Spotify.notFoundSongs)
                    Console.WriteLine(item);
            }

            System.Console.WriteLine(songs.Length);
            System.Console.WriteLine(Spotify.notFoundSongs.Count);

        }

        private static void AssignArrays(string path)
        {
            // Read Json
            string jsonFromFile;
            using (var reader = new StreamReader(path))
            {
                jsonFromFile = reader.ReadToEnd();
            }

            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonFromFile);

            songs = dictionary["songs"].ToArray();

            if (dictionary["unsureSongs"].Count != 0)
            {
                unsureSongs = dictionary["unsureSongs"].ToArray();
            }
        }

        private static string GetPlaylistname()
        {
            Console.Write("\nGive your Playlist a Name: ");
            string name = Console.ReadLine();

            Console.WriteLine("This might take a while depending on the YouTube Playlist.");
            Console.WriteLine("Loading...");
            return name;
        }
        private static void CreatePythonConnection(string pythonPath, string url)
        {
            // Create Process Info
            var psi = new ProcessStartInfo();
            psi.FileName = pythonPath;

            // Provide script and arguments
            var script = "YouTube_DLApi.py";
            var playlistUrl = url;

            psi.Arguments = $"\"{script}\" \"{playlistUrl}\"";

            // Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var process = Process.Start(psi);
            process.WaitForExit();
        }
    }
}
