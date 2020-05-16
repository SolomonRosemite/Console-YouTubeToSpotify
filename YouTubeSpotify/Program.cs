using System.Collections.Generic;
using System;

namespace YouTubeSpotify
{
    class Program
    {
        public static string[] songs;
        public static string[] unsureSongs;

        static void Main(string[] args)
        {
            songs = new string[]
            {
                "Mask Off+Future",
                "Space Cadet+Metro Boomin",
            };

            unsureSongs = new string[]
            {
                // "This is How Easy It Is to Lie With Statistics;This is How Easy It Is to Lie With Statistics"
            };

            Spotify.FinishPlaylist("Bank", songs);

            if (unsureSongs != null)
                Spotify.AddUnsureSongs(unsureSongs);

            if (Spotify.notFoundSongs.Count != 0)
            {
                Console.WriteLine("Songs that couldn't be added:\n");

                foreach (var item in Spotify.notFoundSongs)
                    Console.WriteLine(item);
            }
        }
    }
}
