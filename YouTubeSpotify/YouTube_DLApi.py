import youtube_dl

songs = []

playlist_url = ''

playlist = youtube_dl.YoutubeDL({}).extract_info(playlist_url, download=False)

print("\nSongs:\n")

for song in playlist['entries']:
    try:
        songs.append(song["track"]+ "+" +song["artist"])
    except:
        songs.append(song["title"])

print(songs)