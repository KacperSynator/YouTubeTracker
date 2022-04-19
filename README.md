# YouTubeTracker
![image](https://user-images.githubusercontent.com/62207289/163983553-cf482114-d68e-4a65-b743-c5850afee143.png)
## Description

YouTubeTracker is a window app which allows user to search for videos, play videos, create local playlists and calculate playlist duration.   
App uses **Youtube Data Api v3** to search and aquire videos data (title, embed html, id, thumbnail url, duration). 
**Local database** is used to store playlists and theirs videos.

Note that every api requests cost quota. Default daily quota is 10k. This app consumes 101 quota for every succesfull search (100 for search list and 1 for video list).
More information about quota can be found here: https://developers.google.com/youtube/v3/getting-started#quota.
## Getting Started
* Genarate Youtube Data Api key (instruction: https://developers.google.com/youtube/v3/getting-started#before-you-start first 3 steps),  
 Outh 2.0 is not required.

* In project main directory add file API_key.txt with generated api key.

## Gui Legend
![gui_legend](https://user-images.githubusercontent.com/62207289/164070673-b62fd8f2-44ea-46f7-b2f1-17177538ba1e.png)
1. Text box for typing search phrase also used for typing new playlist name.  
2. Search button executes search with given phrase.
3. Text box for typing max search results from range (1..50).
4. View selection, display videos from selected playlist or results from search.
5. List with videos from selected view.
6. Information about selected playlist (number of videos and duration of playlist).
7. Playlist selection.
8. Create new playlist, name must be typed in phrase box.
9. Delete selected playlist.
10. Play selected video in embed player.
11. Add selected video to selected playlist available only from search view.
12. Remove selected video from selected playlist available only from playlist view.
13. Embed video player.

## Authors

Contributors names and contact info
* Kacper Synator  (https://github.com/KacperSynator)
* Paweł Potoczek (https://github.com/PPotoczek)


## License

This project is licensed under the MIT License - see the LICENSE file for details
