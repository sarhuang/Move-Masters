using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using TMPro;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
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
    float pwoerMissAmount = 0.06f;
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
    public Slider powerBar;
    public Color highPowerColor;
    public Color lowPowerColor;

    static SerialPort serialPort = new("COM10", 9600);
    static bool serialPortError = false;
    static KeyCode keyToPress = KeyCode.None;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManger start!");
        instance = this;
        currentMultiplier = 1;
        UpdateScoreText();
        UpdateStreak(0);
        UpdatePower(0.5f);

        GameObject ns = GameObject.FindWithTag("NoteSpawner");
        if (ns != null) {
            noteSpawner = ns.GetComponent<NoteSpawner>();
            beatScoller = ns.GetComponent<BeatScroller>();
        }

        try
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            serialPort.Open();
            if (serialPort.IsOpen)
            {
                Debug.Log("Serial port is open!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Serial Port Open Error: {ex.Message}");
            serialPortError = true;
        }

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
                noteSpawner.GetAudioSource().Stop();
                noteSpawner.hasStarted = false;
                EndGame();
            }
        }

        ArduinoToKeyVal();
    }

    public void EndGame() {
        resultsScreen.SetActive(true);
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
        noteSpawner.hasStarted = true;
        noteSpawner.GetAudioSource().Play();
    }

    public void ArduinoToKeyVal()
    {
        if (serialPortError) {
            //TODO: Read input from keyboard instead
            return;
        }

        serialPort.ReadTimeout = 1;
        serialPort.DtrEnable = true;

        try
        {
            string dataFromArduinoString = serialPort.ReadLine();
            char keyCodeValue;
            if (char.TryParse(dataFromArduinoString, out keyCodeValue))
            {
                keyToPress = (KeyCode)keyCodeValue;
            }
            else
            {
                keyToPress = KeyCode.None;
            }
        }
        catch (TimeoutException e)
        {
        }
    }

    public static KeyCode GetKeyVal()
    {
        return keyToPress;
    }


    public void NoteHit()
    {
        Debug.Log("Hit on time");
        // if (currentMultiplier - 1 < multiplierThresholds.Length)
        // {
        //     multiplierTracker++;
        //     if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
        //     {
        //         multiplierTracker = 0;
        //         currentMultiplier++;
        //     }
        // }
        UpdateStreak(currentStreak+1);
        UpdatePower(powerVal+powerHitIncreaseAmount);
        UpdateScoreText();
    }

    public void UpdateScoreText() {
        scoreText.text = currentScore.ToString(scorePadding);
    }

    public void UpdatePower(float val) {
        powerVal = val;
        powerBar.value = powerVal;
        powerBar.fillRect.GetComponent<Image>().color = Color.Lerp(lowPowerColor, highPowerColor, powerVal);
    }

    public void UpdateStreak(int val) {
        int threshold;

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
        UpdatePower(powerVal-pwoerMissAmount);
        missedHits++;
    }

    public void SetNoteSpawner(NoteSpawner ns) {
        noteSpawner = ns;
        beatScoller = ns.GetComponent<BeatScroller>();
    }

    public void SetGameMode(string mode) {
        switch (mode) {
            case "ddr":
                ddrMode.SetActive(true);
                piuMode.SetActive(false);
                mixMode.SetActive(false);
                break;
            case "piu":
                ddrMode.SetActive(false);
                piuMode.SetActive(true);
                mixMode.SetActive(false);
                break;
            case "mix":
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
}
