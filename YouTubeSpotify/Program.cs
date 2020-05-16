using System.Collections.Generic;
using System;

using NYoutubeDL;

namespace YouTubeSpotify
{
    class Program
    {
        public static string[] songs;

        static void Main(string[] args)
        {
            songs = new string[]
            {
                // "Mask Off+Future",
                // "Space Cadet+Metro Boomin",
            };

            Spotify.FinishPlaylist("Bank", songs);

            if (Spotify.notFoundSongs.Count != 0)
            {
                Console.WriteLine("Songs that couln't be added: ");

                foreach (var item in Spotify.notFoundSongs)
                    Console.WriteLine(item);
            }
        }
    }
}
