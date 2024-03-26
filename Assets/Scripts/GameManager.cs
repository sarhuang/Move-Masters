using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource song;
    public bool startSong;
    public BeatScoller beatScoller;
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


    // Start is called before the first frame update
    void Start()
    {
       instance = this;
       scoreText.text = "Score: 0";
       currentMultiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(!startSong){
            if(Input.anyKeyDown){
                startSong = true;
                beatScoller.hasStarted = true;
                song.Play();
            }
        }
    }

    public void NoteHit(){
        Debug.Log("Hit on time");
        if(currentMultiplier - 1 < multiplierThresholds.Length){
            multiplierTracker++;
            if(multiplierThresholds[currentMultiplier - 1] <= multiplierTracker){
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        multiplierText.text = "Multipler: x" + currentMultiplier;
        scoreText.text = "Score: " + currentScore;
    }

    public void NormalHit(){
        currentScore += scorePerNote * currentMultiplier;
        NoteHit();

    }

    public void GoodHit(){
        currentScore += scorePerGoodNote * currentMultiplier;
        NoteHit();
    }

    public void PerfectHit(){
        currentScore += scorePerPerfectNote * currentMultiplier;
        NoteHit();
    }

    public void NoteMissed(){
        Debug.Log("Missed note");
        currentMultiplier = 1;
        multiplierTracker = 0;
        multiplierText.text = "Multipler: x" + currentMultiplier;
    }
}
