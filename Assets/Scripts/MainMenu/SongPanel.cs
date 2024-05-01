using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SongPanel : MonoBehaviour
{
    string songName = null;
    string songLocation = null;
    public TMP_Text SongNameText;
    public TMP_Text SongLengthText;
    public Image PanelRef;
    public Image ImageRef;
    public Button ButtonRef;
    public AudioClip musicClip;
    public AudioSource audioSource;
    int musicClipStartTime = 0;

    public void SetSongLocation(string song) {
        songLocation = song;
    }

    public void SetSongName(string name) {
        songName = name;
        SongNameText.text = name;
    }

    public void SetMusicLocation(string location, int duration) {
        musicClip = Resources.Load<AudioClip>($"music/{location.Trim()}");
        musicClipStartTime = duration;

        if (musicClip != null && audioSource != null) {
            audioSource.clip = musicClip;
            audioSource.time = musicClipStartTime;
            SongLengthText.text = ConvertSecondsToTime(audioSource.clip.length);
        }
    }

    string ConvertSecondsToTime(float time) {
        TimeSpan t = TimeSpan.FromSeconds(time);
        return String.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);
    }

    public void PlayMusicClip() {
        if (musicClip != null && audioSource != null) {
            audioSource.clip = musicClip;
            audioSource.time = musicClipStartTime;
            audioSource.Play();
        }
    }

    public void StopMusicClip() {
        if (audioSource != null) {
            audioSource.Stop();
            audioSource.clip = musicClip;
            audioSource.time = musicClipStartTime;
        }
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
        string filepath = $@"Song-Files/{songLocation.Trim()}";
        Debug.Log(songLocation);
        if (songLocation == null) {
            Debug.LogError("SongPanel set up incorrectly!");
            return;
        }

        TextAsset songFile = Resources.Load<TextAsset>(filepath);
        if (songFile == null) {
            Debug.LogError($"Unable to load song file {filepath}.");
            return;
        }

        //TODO: Check if note spawner already exists, if it does change the song it loaded
        ns = Instantiate(MainMenuController.m.NoteSpawnerRef).GetComponent<NoteSpawner>();
        //ns.LoadSongFile(songLocation);
        ns.musicGameMode = MainMenuController.m.GetSelectedGameMode();
        switch(ns.musicGameMode){
            case GameMode.DDR:
                songLocation = songLocation.Trim();
                songLocation += "_ddr";
                break;
            case GameMode.PIU:
                songLocation = songLocation.Trim();
                songLocation += "_piu";
                break;
        }
        Debug.Log("new song location: " + songLocation);
        ns.LoadSongFile(songLocation);
        ns.difficulty = MainMenuController.m.GetSelectedDifficulty();
        MainMenuController.m.CleanUpEvents();
        DontDestroyOnLoad(ns.gameObject);
        SceneManager.LoadScene(1);
    }

    public void SetImageIcon(string imageLocation) {
        Sprite icon = Resources.Load<Sprite>($"song-icons/{imageLocation.Trim()}");
        if (icon == null) {
            print($"Unable to load icon at location {imageLocation}");
        } else {
            ImageRef.sprite = icon;
        }
    }
}
