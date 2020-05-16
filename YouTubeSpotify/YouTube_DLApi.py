import youtube_dl
import json
import sys
import re

playlist_url = sys.argv[1]

def get_song_of_title(title):
    if '-' not in title:
        if '"' not in title:
            return title

        result = re.search('"(.*)"', title)

        return title.split(' ')[0] + "+" + result.group(1)

    return title.split('-')[0]+ '+' + (title.split('-')[1]).split(' ')[1]


songs = []
failed = []

playlist = youtube_dl.YoutubeDL({}).extract_info(playlist_url, download=False)

print("\nSongs:\n")

for song in playlist['entries']:
    try:
        songs.append(song["track"]+ "+" +song["artist"])
    except:
        failed.append(get_song_of_title(song["title"]) + ";" + song["title"])


print(json.dumps(songs))
print(':Split here:')
print(json.dumps(failed))