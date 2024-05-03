using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using TMPro;
//using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool startSong;
    public BeatScroller beatScoller = null;
    public NoteSpawner noteSpawner = null;

    public static GameManager instance;

    public int currentScore = 0;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    public int currentMultiplier;
    public int multiplierTracker;
    int currentStreak = 0;
    float powerVal = 0.5f;
    float powerHitIncreaseAmount = 0.02f;
    float powerMissAmount = 0.06f;
    float endPitch = 0.1f;
    float endPitchDecreaseSpeed = 1f;
    float resultScreenFadeSpeed = 3f;
    bool endSongFail = false; //Used to make the song fade out when the user fails
    bool fadeInResultScreen = false;
    Image resultScreenImage;
    public int[] multiplierThresholds;

    public float totalNotes;
    public float normalHits;
    public float goodHits;
    public float perfectHits;
    public float missedHits;
    [Header("Gamemode Refs")]
    public GameObject mixMode;
    public GameObject ddrMode;
    public GameObject piuMode;
    public Transform buttonSpawnLocation;

    [Header("UI Objects")]
    public Text percentHitText;
    public Text normalsText, goodsText, perfectsText, missesText, rankText, finalScoreText;
    public GameObject resultsScreen;
    public TMP_Text scoreText;
    string scorePadding = "00000000";
    public TMP_Text multiplierText;
    public TMP_Text streakText;
    float defaultStreakFontSize;
    float streakFontGrowSize = 5;
    float streakFontGrowSpeed = 20;
    public Slider powerBar;
    public Color highPowerColor;
    public Color lowPowerColor;

    static List<KeyCode> keyToPress;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManger start!");
        instance = this;
        keyToPress = new List<KeyCode>();
        currentMultiplier = 1;
        defaultStreakFontSize = streakText.fontSize;
        resultsScreen.SetActive(false);
        resultScreenImage = resultsScreen.GetComponent<Image>();;
        UpdateScoreText();
        UpdateStreak(0);
        UpdatePower(0.5f);

        GameObject ns = GameObject.FindWithTag("NoteSpawner");
        if (ns != null) {
            noteSpawner = ns.GetComponent<NoteSpawner>();
            beatScoller = ns.GetComponent<BeatScroller>();
        }
        noteSpawner.GetAudioSource().pitch = 1;
        noteSpawner.GetAudioSource().volume = 1;

        SetGameMode(noteSpawner.musicGameMode);
        StartSong();
    }

    // Update is called once per frame
    void Update()
    {
        if (startSong)
        {
            if (!noteSpawner.GetAudioSource().isPlaying && !resultsScreen.activeInHierarchy)
            {
                //This means the song naturally ended
                EndGame();
            }

            if (powerVal <= 0) {
                //noteSpawner.GetAudioSource().Stop();
                endSongFail = true;
                noteSpawner.hasStarted = false;
                EndGame();
            }
        }

        if (endSongFail) {
            if (noteSpawner.GetAudioSource().pitch > endPitch) {
                noteSpawner.GetAudioSource().pitch = noteSpawner.GetAudioSource().pitch - endPitchDecreaseSpeed*Time.deltaTime;
                noteSpawner.GetAudioSource().volume = noteSpawner.GetAudioSource().volume - Time.deltaTime;
            } else {
                noteSpawner.GetAudioSource().Stop();
            }
        }

        if (fadeInResultScreen && resultScreenImage.color.a < 1) {
            SetResultFade(resultScreenImage.color.a + resultScreenFadeSpeed*Time.deltaTime);
        }

        ArduinoToKeyVal();
        ApplyPowerbarRainbowEffect();
        ApplyStreakTextAnimation();
    }

    void SetResultFade(float a) {
        Image[] childrenImages = resultsScreen.GetComponentsInChildren<Image>();
        TMP_Text[] allTextTMP = resultsScreen.GetComponentsInChildren<TMP_Text>();
        Text[] allText = resultsScreen.GetComponentsInChildren<Text>();

        resultScreenImage.color = new Color(resultScreenImage.color.r, resultScreenImage.color.g, resultScreenImage.color.b, a);
        foreach (Image i in childrenImages) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        }

        foreach (TMP_Text i in allTextTMP) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        }

        foreach (Text i in allText) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, a);
        }
    }

    public void EndGame() {
        if (!fadeInResultScreen) {
            SetResultFade(0);
            resultsScreen.SetActive(true);
            fadeInResultScreen = true;
        }
        normalsText.text = normalHits.ToString();
        goodsText.text = goodHits.ToString();
        perfectsText.text = perfectHits.ToString();
        missesText.text = missedHits.ToString();
        totalNotes = normalHits + goodHits + perfectHits + missedHits;
        float percentHit = (normalHits + goodHits + perfectHits) / totalNotes * 100f;
        percentHitText.text = percentHit.ToString("F1") + "%";

        //Destroy all existing notes
        noteSpawner.DestroyAllNotes();

        string rankVal = "F";
        if (percentHit > 95)
        {
            rankVal = "S";
        }
        else if (percentHit > 85)
        {
            rankVal = "A";
        }
        else if (percentHit > 70)
        {
            rankVal = "B";
        }
        else if (percentHit > 55)
        {
            rankVal = "C";
        }
        else if (percentHit > 40)
        {
            rankVal = "D";
        }
        rankText.text = rankVal;
        finalScoreText.text = currentScore.ToString();
    }

    public void StartSong() {
        startSong = true;
        beatScoller.hasStarted = true;
        noteSpawner.LoadSpawnTimes();
        noteSpawner.hasStarted = true;
        noteSpawner.GetAudioSource().Play();
    }

    public void ArduinoToKeyVal()
    {
        if (!SerialController.SerialPortIsActive()) {
            //TODO: Read input from keyboard instead
            return;
        }

        keyToPress.Clear();

        List<ButtonMap> dataFromArduinoString = SerialController.ReadLine();
        foreach (ButtonMap k in dataFromArduinoString) {
            keyToPress.Add((KeyCode)k);
        }
    }

    public static List<KeyCode> GetKeyVal()
    {
        return keyToPress;
    }


    public void NoteHit()
    {
        Debug.Log("Hit on time");
        UpdateStreak(currentStreak+1);
        UpdatePower(powerVal+powerHitIncreaseAmount);
        UpdateScoreText();
    }

    public void UpdateScoreText() {
        scoreText.text = currentScore.ToString(scorePadding);
    }

    public void UpdatePower(float val) {
        if (val > 1) {
            powerVal = 1;
        } else {
            powerVal = val;
        }

        powerBar.value = powerVal;
        if (powerVal < 1) {
            powerBar.fillRect.GetComponent<Image>().color = Color.Lerp(lowPowerColor, highPowerColor, powerVal);
        }
    }

    public void ApplyPowerbarRainbowEffect() {
        if (powerVal >= 1) {
            powerBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.green, Color.cyan, Mathf.PingPong(Time.time, 1));
        }
    }

    public void ApplyStreakTextAnimation() {
        if (streakText.fontSize != defaultStreakFontSize) {
            streakText.fontSize = Mathf.Lerp(streakText.fontSize, defaultStreakFontSize, 0.5f*Time.deltaTime*streakFontGrowSpeed);
        }
    }

    public void UpdateStreak(int val) {
        int threshold;

        streakText.fontSize = defaultStreakFontSize+streakFontGrowSize;
        currentStreak = val;
        if (currentStreak == 0) {
            currentMultiplier = 1;
        } else {
            //This has to be done in reverse so we get the right multiplier
            for (int i = multiplierThresholds.Length-1; i > 0; i--) {
                threshold = multiplierThresholds[i];
                if (currentStreak >= threshold) {
                    currentMultiplier = i + 1;
                    break;
                }
            }
        }

        multiplierText.text = $"x{currentMultiplier}";
        streakText.text = $"{currentStreak} streak";
    }

    public void NormalHit()
    {
        currentScore += scorePerNote * currentMultiplier;
        NoteHit();
        normalHits++;

    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        NoteHit();
        goodHits++;
    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        NoteHit();
        perfectHits++;
    }

    public void NoteMissed()
    {
        Debug.Log("Missed note");
        UpdateStreak(0);
        multiplierTracker = 0;
        UpdatePower(powerVal-powerMissAmount);
        missedHits++;
    }

    public void SetNoteSpawner(NoteSpawner ns) {
        noteSpawner = ns;
        beatScoller = ns.GetComponent<BeatScroller>();
    }

    public void SetGameMode(GameMode mode) {
        switch (mode) {
            case GameMode.DDR:
                ddrMode.SetActive(true);
                piuMode.SetActive(false);
                mixMode.SetActive(false);
                break;
            case GameMode.PIU:
                ddrMode.SetActive(false);
                piuMode.SetActive(true);
                mixMode.SetActive(false);
                break;
            case GameMode.MIX:
                ddrMode.SetActive(false);
                piuMode.SetActive(false);
                mixMode.SetActive(true);
                break;
        }
    }

    public void ReturnToMainMenu() {
        Destroy(noteSpawner.gameObject); //This prevents a bug that can happen if we then try to go select another song after exiting
        SceneManager.LoadScene(0);
    }

    public void PlaySongAgain() {
        SceneManager.LoadScene(1);
    }
}
