using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController m;
    public GameObject TitleScreen;
    public GameObject SongSelectionScreen;
    public TMP_Text TitleText;
    public GameObject SongPanelRef;
    public GameObject NoteSpawnerRef;
    LinkedList<AnimatedObject> animatedObjects;
    List<AnimatedObject> finishedAnimations;
    List<SongPanel> allSongs;
    int songSelectionIndex = 0;
    readonly int songDisplacement = 400; //Distance between song panels

    void Start() {
        m = this;
        Vector2 titleStartPos = new Vector2(TitleText.transform.position.x, TitleText.transform.position.y+400);
        Vector2 titleEndPos = new Vector2(TitleText.transform.position.x, TitleText.transform.position.y);

        animatedObjects = new LinkedList<AnimatedObject>();
        finishedAnimations = new List<AnimatedObject>();
        allSongs = new List<SongPanel>();

        TitleScreen.SetActive(true);
        SongSelectionScreen.SetActive(false);

        AnimateTranslateObject(TitleText.gameObject, titleStartPos, titleEndPos, 2f);
        LoadAllSongs();
    }

    void Update() {
        bool animFinished;
        finishedAnimations.Clear();

        //Update any animations we currently have
        foreach (AnimatedObject ao in animatedObjects) {
            animFinished = ao.UpdateAnimationFrame();
            if (animFinished) {
                finishedAnimations.Add(ao);
            }
        }

        //Remove any finished animations from the linked list
        foreach (AnimatedObject ao in finishedAnimations) {
            animatedObjects.Remove(ao);
        }

        //TODO: Replace this with serial input from the board
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ChangeSongSelection(songSelectionIndex+1);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ChangeSongSelection(songSelectionIndex-1);
        }
    }

    //Changes the current song panel on the song selection page
    void ChangeSongSelection(int index) {
        SongPanel song;

        if (index < 0) return;
        if (index >= allSongs.Count) return;

        songSelectionIndex = index;
        for (int i = 0; i < allSongs.Count; i++) {
            song = allSongs[i];
            if (songSelectionIndex == i) {
                SetSongPanelInFocus(song);
            } else {
                SetSongPanelOutOfFocus(song);
            }

            song.transform.localPosition = new Vector2((i-songSelectionIndex)*songDisplacement, 0);
        }
    }

    void LoadAllSongs() {
        //Only call this once, as it will otherwise create duplicate objects
        string[] parsedString;
        string songName, songLocation, imageLocation, musicLocation;
        float songStartTime;
        TextAsset[] allSongs = Resources.LoadAll("song-selection", typeof(TextAsset)).Cast<TextAsset>().ToArray();

        foreach (TextAsset song in allSongs) {
            Debug.Log($"Parsing song {song.name}");
            parsedString = song.text.Split("\n");
            if (parsedString.Length < 5) {
                Debug.LogWarning($"Song File {song.name} is in the wrong format, skipping song...");
                continue;
            }

            songName = parsedString[0];
            songLocation = parsedString[1];
            imageLocation = parsedString[2];
            musicLocation = parsedString[3];
            songStartTime = (float)Convert.ToDecimal(parsedString[4]);

            CreateSongPanel(songName, songLocation, imageLocation);
        }

        //SetSongDisplayPosition();
        ChangeSongSelection(0);
    }

    void SetSongPanelInFocus(SongPanel song) {
        song.transform.localScale = new Vector3(1f, 1f, 1f);
        song.SetPanelFade(0.4f);
        song.SetImageFade(1f);
        song.SetSongNameFade(1f);
        song.SetButtonState(true);
    }

    void SetSongPanelOutOfFocus(SongPanel song) {
        song.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        song.SetPanelFade(0.1f);
        song.SetImageFade(0.5f);
        song.SetSongNameFade(0.5f);
        song.SetButtonState(false);
    }

    void CreateSongPanel(string songName, string songLocation, string imageLocation) {
        SongPanel songPanel = Instantiate(SongPanelRef, SongSelectionScreen.transform).GetComponent<SongPanel>();
        songPanel.SetSongName(songName);
        songPanel.SetSongLocation(songLocation);
        allSongs.Add(songPanel);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenSongSelectionScreen() {
        TitleScreen.SetActive(false);
        SongSelectionScreen.SetActive(true);
    }

    public void AnimateTranslateObject(GameObject obj, Vector2 startPos, Vector2 endPos, float speed) {
        AnimatedObject ao = new AnimatedObject(obj, AnimationType.TRANSLATE);
        ao.startPos = startPos;
        ao.endPos = endPos;
        ao.speed = speed;

        obj.transform.position = startPos;
        animatedObjects.AddLast(ao);
    }
}

public class AnimatedObject {
    public GameObject gameObj;
    public Vector2 startPos;
    public Vector2 endPos;
    public float speed;
    public AnimationType animationType;
    public static float distanceMOE = 0.05f; //Margin of error

    public AnimatedObject(GameObject gameObj, AnimationType animationType) {
        this.gameObj = gameObj;
        this.animationType = animationType;
    }

    public bool UpdateAnimationFrame() {
        switch (animationType) {
            case AnimationType.TRANSLATE:
                return UpdatePosition();
        }

        return true;
    }

    bool UpdatePosition() {
        //Returns true if it has reached the target
        gameObj.transform.position = Vector2.Lerp(gameObj.transform.position, endPos, speed*Time.deltaTime);
        if (Mathf.Abs(Vector2.Distance(gameObj.transform.position, endPos)) < distanceMOE) {
            return true;
        }
        return false;
    }
}

public enum AnimationType {
    NONE,
    TRANSLATE,
    FADE_IN
}