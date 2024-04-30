using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController m;
    public GameObject TitleScreen;
    public GameObject SongSelectionScreen;
    public Image TitleLogo;
    public GameObject SongPanelRef;
    public GameObject NoteSpawnerRef;
    public TMP_Text DifficultyText;
    public TMP_Text ModeText;

    LinkedList<AnimatedObject> animatedObjects;
    List<AnimatedObject> finishedAnimations;
    List<SongPanel> allSongs;
    int songSelectionIndex = 0;
    Difficulty selectedDifficulty;
    GameMode selectedGameMode;
    readonly int songDisplacement = 400; //Distance between song panels

    void Awake() {
        SerialController.SerialError += c_SerialError;
        SerialController.ButtonPress += c_ButtonPressed;
    }

    void Start() {
        m = this;
        Vector2 titleStartPos = new Vector2(TitleLogo.transform.position.x, TitleLogo.transform.position.y+400);
        Vector2 titleEndPos = new Vector2(TitleLogo.transform.position.x, TitleLogo.transform.position.y);

        animatedObjects = new LinkedList<AnimatedObject>();
        finishedAnimations = new List<AnimatedObject>();
        allSongs = new List<SongPanel>();

        TitleScreen.SetActive(true);
        SongSelectionScreen.SetActive(false);

        SetDifficulty(Difficulty.EASY);

        AnimateTranslateObject(TitleLogo.gameObject, titleStartPos, titleEndPos, 2f);
        LoadAllSongs();
    }

    void c_SerialError(object sender, ButtonPressEventArgs e) {
        //Debug.LogError("Serial Error received!");
    }

    void c_ButtonPressed(object sender, ButtonPressEventArgs e) {
        List<ButtonMap> pressed = e.buttons;

        foreach (ButtonMap b in pressed) {
            switch (b) {
                case ButtonMap.RIGHT_ARROW:
                    NextSong();
                    break;
                case ButtonMap.LEFT_ARROW:
                    PreviousSong();
                    break;
            }
        }
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
            NextSong();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            PreviousSong();
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            IncrementDifficulty();
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            IncrementGameMode();
        }

        if (SerialController.SerialPortIsActive()) {
            //Since we are using events, we just need to read the events to respond to input
            //Yes it's overkill and we could just interpret the presses here, but I wanted to learn events
            SerialController.ReadLine();
        }
    }

    public void NextSong() {
        ChangeSongSelection(songSelectionIndex+1);
    }

    public void PreviousSong() {
        ChangeSongSelection(songSelectionIndex-1);
    }

    public void IncrementDifficulty() {
        int nextDifficulty = (int)selectedDifficulty+1;
        if (nextDifficulty >= 3) {
            nextDifficulty = 0;
        }

        SetDifficulty((Difficulty)nextDifficulty);
    }

    public void IncrementGameMode() {
        int nextMode = (int)selectedGameMode+1;
        if (nextMode >= 3) {
            nextMode = 0;
        }

        SetGameMode((GameMode)nextMode);
    }

    public void SetDifficulty(Difficulty dif) {
        selectedDifficulty = dif;

        switch (dif) {
            case Difficulty.EASY:
                DifficultyText.text = "Easy";
                break;
            case Difficulty.MEDIUM:
                DifficultyText.text = "Medium";
                break;
            case Difficulty.HARD:
                DifficultyText.text = "Hard";
                break;
        }
    }

    public void SetGameMode(GameMode gm) {
        selectedGameMode = gm;

        switch (gm) {
            case GameMode.FULL:
                ModeText.text = "Full";
                break;
            case GameMode.DDR:
                ModeText.text = "DDR";
                break;
            case GameMode.PIU:
                ModeText.text = "PIU";
                break;
        }
    }

    public void CleanUpEvents() {
        //This must be called before any scene changes
        SerialController.SerialError -= c_SerialError;
        SerialController.ButtonPress -= c_ButtonPressed;
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
        int songStartTime;
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
            songStartTime = (int)Convert.ToInt32(parsedString[4]);

            CreateSongPanel(songName, songLocation, imageLocation, songStartTime, musicLocation);
        }

        //SetSongDisplayPosition();
        //ChangeSongSelection(0);
    }

    void SetSongPanelInFocus(SongPanel song) {
        song.transform.localScale = new Vector3(1f, 1f, 1f);
        song.SetPanelFade(0.4f);
        song.SetImageFade(1f);
        song.SetSongNameFade(1f);
        song.SetButtonState(true);
        song.PlayMusicClip();
    }

    void SetSongPanelOutOfFocus(SongPanel song) {
        song.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        song.SetPanelFade(0.1f);
        song.SetImageFade(0.5f);
        song.SetSongNameFade(0.5f);
        song.SetButtonState(false);
        song.StopMusicClip();
    }

    void CreateSongPanel(string songName, string songLocation, string imageLocation, int songStartTime, string musicLocation) {
        SongPanel songPanel = Instantiate(SongPanelRef, SongSelectionScreen.transform).GetComponent<SongPanel>();
        songPanel.SetSongName(songName);
        songPanel.SetSongLocation(songLocation);
        songPanel.SetImageIcon(imageLocation);
        songPanel.SetMusicLocation(musicLocation, songStartTime);
        allSongs.Add(songPanel);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenSongSelectionScreen() {
        SerialController.SerialPortIsActive();
        TitleScreen.SetActive(false);
        SongSelectionScreen.SetActive(true);
        ChangeSongSelection(0);
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