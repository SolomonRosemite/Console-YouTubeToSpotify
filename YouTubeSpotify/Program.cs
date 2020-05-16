using System.Diagnostics;
using System;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            string name = GetPlaylistname();

            ProcessStartInfo process = CreatePythonConnection(pathToPython, playlistUrl);
            string results = GetResults(process);
            int unsureSongsStartIndex;

            results = results.Substring(results.IndexOf("Songs:") + 6);
            string songsResults = results.Remove(unsureSongsStartIndex = results.IndexOf(":Split here:"));

            results = results.Substring(unsureSongsStartIndex + 12);

            JArray songsList = (JArray)JsonConvert.DeserializeObject(songsResults);
            songs = songsList.ToObject<string[]>();

            JArray unsureSongsList = (JArray)JsonConvert.DeserializeObject(results);

            unsureSongs = unsureSongsList.Count != 0 ? unsureSongsList.ToObject<string[]>() : null;

            Spotify.FinishPlaylist(name, songs);

            if (unsureSongs != null)
                Spotify.AddUnsureSongs(unsureSongs);

            if (Spotify.notFoundSongs.Count != 0)
            {
                Console.WriteLine("Songs that couldn't be added:\n");

                foreach (var item in Spotify.notFoundSongs)
                    Console.WriteLine(item);
            }

            Console.WriteLine("Ratio: " + (((float)(songs.Length - Spotify.notFoundSongs.Count) / (float)songs.Length) * 100).ToString());
            Console.WriteLine("Done");
        }

        private static string GetPlaylistname()
        {
            string name = Console.ReadLine();

            Console.WriteLine("This might take a while... depending on the Playlist.");
            Console.WriteLine("Loading...");
            return name;
        }

        private static ProcessStartInfo CreatePythonConnection(string pythonPath, string url)
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
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            return psi;
        }

        private static string GetResults(ProcessStartInfo processStartInfo)
        {
            var results = "";

            using (var process = Process.Start(processStartInfo))
                results = process.StandardOutput.ReadToEnd();

            return results;
        }
    }
}
