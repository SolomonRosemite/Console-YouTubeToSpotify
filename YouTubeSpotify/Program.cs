using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace YouTubeSpotify
{
    class Program
    {
        public static string[] songs;
        public static string[] unsureSongs;

        static void Main(string[] args)
        {
            ProcessStartInfo process = CreatePythonConnection
            (
                @"C:\Python38\python.exe",
                "https://www.youtube.com/playlist?list=PLlYKDqBVDxX2KziuzmDlNhLpJjgLktB53"
            );

            string results = GetResults(process);

            // songs = new string[]
            // {
            //     "Mask Off+Future",
            //     "Space Cadet+Metro Boomin",
            // };

            // unsureSongs = new string[]
            // {
            //     // "This is How Easy It Is to Lie With Statistics;This is How Easy It Is to Lie With Statistics"
            // };

            // Spotify.FinishPlaylist("Bank", songs);

            // if (unsureSongs != null)
            //     Spotify.AddUnsureSongs(unsureSongs);

            // if (Spotify.notFoundSongs.Count != 0)
            // {
            //     Console.WriteLine("Songs that couldn't be added:\n");

            //     foreach (var item in Spotify.notFoundSongs)
            //         Console.WriteLine(item);
            // }
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
