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
    private static FullPlaylist playlist;

    private static bool completed = false;

    public static void FinishPlaylist(string nameOfPlaylist, string[] songs)
    {
        CredentialsAuth(Credentials.ClientID);

        while (completed == false)
            System.Threading.Thread.Sleep(1000);

        // Creates a Spotify Playlist
        playlist = CreatePlaylist(profile.Id, nameOfPlaylist);

        // Adds all songs to a list
        List<string> tracks = GetTracks(songs);
        AddTracksToPlaylist(playlist, tracks);
    }

    public static void AddUnsureSongs(string[] songs)
    {
        List<string> tracks = GetTracks(songs, true);
        if (tracks.Count != 0)
        {
            AddTracksToPlaylist(playlist, tracks);
        }
    }

    static FullPlaylist CreatePlaylist(string id, string name)
    {
        var playlist = spotify.CreatePlaylist(id, name);
        if (playlist.HasError())
        {
            return null;
        }

        return playlist;
    }
    static bool AddTracksToPlaylist(FullPlaylist playlist, List<string> uris)
    {
        ErrorResponse response = spotify.AddPlaylistTracks(playlist.Id, uris);
        if (response.HasError())
        {
            return false;
        }
        return true;
    }
    static FullTrack GetTrack(string tag, bool unsure = false)
    {
        SearchItem track = spotify.SearchItems(tag, SearchType.Track, 1);
        if (track.HasError() || track.Tracks.Items.Count == 0)
        {
            if (unsure)
                notFoundSongs.Add(tag.Split(';')[1]);
            else
                notFoundSongs.Add(tag.Replace('+', ' '));

            return null;
        }

        return track.Tracks.Items[0];
    }
    static List<string> GetTracks(string[] tags, bool unsure = false)
    {
        List<string> trackUris = new List<string>();
        foreach (var item in tags)
        {
            var song = GetTrack(item, unsure);
            if (song == null)
                continue;
            trackUris.Add(song.Uri);
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