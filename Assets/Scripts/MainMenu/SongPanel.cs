using System.Collections;
using System.Collections.Generic;
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
        }
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
        ns.LoadSongFile(songLocation);
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
