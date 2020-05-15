using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;

using System.Collections.Generic;
using System;

namespace YouTubeSpotify
{
    class Program
    {
        private static SpotifyWebAPI spotify;
        private static PrivateProfile profile;

        private static string[] tags = new string[]
        {
            "Yuck+Logic",
            "Future+Mask Off",
            "44 more+Logic",
            "4 am+Justin stone",
        };

        private static bool completed = false;
        static void Main(string[] args)
        {
            CredentialsAuth(Credentials.ClientID);

            while (completed == false)
            {
                System.Threading.Thread.Sleep(1000 * 1);
            }

            // Creates a Spotify Playlist
            FullPlaylist playlist = CreatePlaylist(profile.Id, "London");

            // Add's all songs to a list
            List<string> tracks = GetTracks(tags);
            AddTracksToPlaylist(playlist, tracks);
        }

        static FullPlaylist CreatePlaylist(string id, string name)
        {
            var playlist = spotify.CreatePlaylist(id, name);
            if (playlist.HasError())
            {
                System.Console.WriteLine(playlist.Error.Message);
                return null;
            }

            return playlist;
        }
        static bool AddTrackToPlaylist(FullPlaylist playlist, FullTrack track)
        {
            ErrorResponse response = spotify.AddPlaylistTrack(playlist.Id, track.Uri);
            if (response.HasError())
            {
                Console.WriteLine(response.Error.Message);
                return false;
            }
            return true;
        }
        static bool AddTracksToPlaylist(FullPlaylist playlist, List<string> uris)
        {
            ErrorResponse response = spotify.AddPlaylistTracks(playlist.Id, uris);
            if (response.HasError())
            {
                Console.WriteLine(response.Error.Message);
                return false;
            }
            return true;
        }
        static FullTrack GetTrack(string tags)
        {
            SearchItem track = spotify.SearchItems(tags, SearchType.Track, 1);
            if (track.HasError())
            {
                System.Console.WriteLine(track.Error.Message);
                return null;
            }
            return track.Tracks.Items[0];
        }
        static List<string> GetTracks(string[] tags)
        {
            List<string> trackUris = new List<string>();
            foreach (var item in tags)
                trackUris.Add(GetTrack(item).Uri);

            return trackUris;
        }
        static void CredentialsAuth(string ClientID)
        {
            ImplicitGrantAuth auth = new ImplicitGrantAuth(
                ClientID,
                "http://localhost:4002",
                "http://localhost:4002",
                Scope.UserLibraryModify | Scope.PlaylistModifyPrivate |
                Scope.PlaylistModifyPublic | Scope.PlaylistReadPrivate |
                Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative |
                Scope.UserLibraryRead | Scope.UserLibraryModify |
                Scope.UserReadPrivate | Scope.UserReadEmail
            );
            auth.AuthReceived += (sender, payload) =>
            {
                auth.Stop();
                spotify = new SpotifyWebAPI()
                {
                    TokenType = payload.TokenType,
                    AccessToken = payload.AccessToken
                };

                profile = spotify.GetPrivateProfile();

                completed = true;
            };
            auth.Start();
            auth.OpenBrowser();
        }
    }
}
