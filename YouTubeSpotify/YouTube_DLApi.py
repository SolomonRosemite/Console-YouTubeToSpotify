from __future__ import unicode_literals
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


ydl_opts = {'ignoreerrors': True}
with youtube_dl.YoutubeDL(ydl_opts) as ydl:
    playlist = ydl.extract_info(playlist_url, download=False)

songs = []
failed = []


for song in playlist['entries']:
    try:
        songs.append(song["track"]+ "+" +song["artist"])
    except:
        try:
            failed.append(get_song_of_title(song["title"]) + ";" + song["title"])
        except:
            pass

data = {}

data['songs'] = songs
data['unsureSongs'] = failed

with open('data.json', 'w') as outfile:
    json.dump(data, outfile)