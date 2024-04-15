using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool startSong;
    public BeatScroller beatScoller = null;
    public NoteSpawner noteSpawner = null;

    public static GameManager instance;

    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;
    public Text scoreText;
    public Text multiplierText;

    public float totalNotes;
    public float normalHits;
    public float goodHits;
    public float perfectHits;
    public float missedHits;

    public GameObject resultsScreen;
    public Text percentHitText, normalsText, goodsText, perfectsText, missesText, rankText, finalScoreText;

    static SerialPort serialPort = new("COM10", 9600);
    static bool serialPortError = false;
    static KeyCode keyToPress = KeyCode.None;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManger start!");
        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;

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

        StartSong();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startSong)
        {
            //TODO: Clean up this if statement
        }
        else
        {
            if (!noteSpawner.GetAudioSource().isPlaying && !resultsScreen.activeInHierarchy)
            {
                resultsScreen.SetActive(true);
                normalsText.text = normalHits.ToString();
                goodsText.text = goodHits.ToString();
                perfectsText.text = perfectHits.ToString();
                missesText.text = missedHits.ToString();
                totalNotes = normalHits + goodHits + perfectHits + missedHits;
                float percentHit = (normalHits + goodHits + perfectHits) / totalNotes * 100f;
                percentHitText.text = percentHit.ToString("F1") + "%";

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
        }

        ArduinoToKeyVal();
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
        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        multiplierText.text = "Multipler: x" + currentMultiplier;
        scoreText.text = "Score: " + currentScore;
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
        currentMultiplier = 1;
        multiplierTracker = 0;
        multiplierText.text = "Multipler: x" + currentMultiplier;
        missedHits++;
    }

    public void SetNoteSpawner(NoteSpawner ns) {
        noteSpawner = ns;
        beatScoller = ns.GetComponent<BeatScroller>();
    }
}
