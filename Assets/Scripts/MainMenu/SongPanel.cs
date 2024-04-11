using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SongPanel : MonoBehaviour
{
    string songName = null;
    string songLocation = null;
    public TMP_Text SongNameText;

    public void SetSongLocation(string song) {
        songLocation = song;
    }

    public void SetSongName(string name) {
        songName = name;
        SongNameText.text = name;
    }

    public void PlaySong() {
        //Create song selector object, change scene
        if (songName == null) {
            Debug.LogWarning("SongPanel set up incorrectly!");
            return;
        }
    }
}
