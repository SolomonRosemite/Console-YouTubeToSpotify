using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;

using System.Collections.Generic;
using System;

static class Spotify
{
    public static List<string> notFoundSongs = new List<string>();

    private static SpotifyWebAPI spotify;
    private static PrivateProfile profile;

    private static bool completed = false;

    public static void FinishPlaylist(string nameOfPlaylist, string[] songs)
    {
        CredentialsAuth(Credentials.ClientID);

        while (completed == false)
            System.Threading.Thread.Sleep(1000);

        // Creates a Spotify Playlist
        FullPlaylist playlist = CreatePlaylist(profile.Id, nameOfPlaylist);

        // Adds all songs to a list
        List<string> tracks = GetTracks(songs);
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
    static FullTrack GetTrack(string tag)
    {
        SearchItem track = spotify.SearchItems(tag, SearchType.Track, 1);
        if (track.HasError())
        {
            notFoundSongs.Add(tag.Replace('+', ' '));
            System.Console.WriteLine(track.Error.Message);
            return null;
        }
        return track.Tracks.Items[0];
    }
    static List<string> GetTracks(string[] tags)
    {
        List<string> trackUris = new List<string>();
        foreach (var item in tags)
        {
            string song = GetTrack(item).Uri;
            if (song == null)
                continue;
            trackUris.Add(song);
        }

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