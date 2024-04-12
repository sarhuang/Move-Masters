using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongPanel : MonoBehaviour
{
    string songName = null;
    string songLocation = null;
    public TMP_Text SongNameText;
    public Image PanelRef;
    public Image ImageRef;
    public Button ButtonRef;

    public void SetSongLocation(string song) {
        songLocation = song;
    }

    public void SetSongName(string name) {
        songName = name;
        SongNameText.text = name;
    }

    public void SetPanelFade(float alpha) {
        PanelRef.color = new Color(PanelRef.color.r, PanelRef.color.g, PanelRef.color.b, alpha);
    }

    public void SetImageFade(float alpha) {
        ImageRef.color = new Color(ImageRef.color.r, ImageRef.color.g, ImageRef.color.b, alpha);
    }

    public void SetSongNameFade(float alpha) {
        SongNameText.color = new Color(SongNameText.color.r, SongNameText.color.g, SongNameText.color.b, alpha);
    }

    public void SetButtonState(bool state) {
        ButtonRef.interactable = state;
    }

    public void PlaySong() {
        //Create song selector object, change scene
        NoteSpawner ns;

        if (songLocation == null) {
            Debug.LogError("SongPanel set up incorrectly!");
            return;
        }

        TextAsset songFile = Resources.Load<TextAsset>(songLocation);
        if (songFile == null) {
            Debug.LogError($"Unable to load song file {songLocation}");
            return;
        }

        //TODO: Check if note spawner already exists, if it does change the song it loaded
        ns = Instantiate(MainMenuController.m.NoteSpawnerRef).GetComponent<NoteSpawner>();
        ns.LoadSongFile(songLocation);
        DontDestroyOnLoad(ns.gameObject);
        SceneManager.LoadScene(1);
    }
}
