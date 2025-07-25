using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner ns;
    public bool hasStarted;
    public GameObject[] notePrefabs;
    public string timingsFileName; // Name of the text file containing timings
    public float songBPM;

    private float[] spawnTimes;
    private string[] noteTypes;

    private float elapsedTime = 0.0f;
    private int currentSpawnIndex = 0;
    int noteSkip = 0; //This is used for the difficulty
    AudioSource audioSource;
    string musicFileName;
    List<NoteObject> spawnedNotes;
    public GameMode musicGameMode;
    public Difficulty difficulty;

    void Awake() {
        //This code checks and makes sure a NoteSpawner doesn't already exist. If it does, it gets destroyed
        GameObject[] check = GameObject.FindGameObjectsWithTag("NoteSpawner");
        foreach (GameObject c in check) {
            if (c != gameObject) {
                //This makes sure it excludes itself from the check
                Destroy(gameObject);
            }
        }

        audioSource = GetComponent<AudioSource>();
        spawnedNotes = new List<NoteObject>();
        //LoadSpawnTimes();
    }

    public void LoadSpawnTimes()
    {
        // Load the text file containing timings from Resources folder
        //Debug.Log("timingsFileName hex: " + System.BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(timingsFileName)));
        //Debug.Log("Expected hex: " + System.BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes("alphabet_songddr")));
        
        TextAsset timingTextAsset = Resources.Load<TextAsset>($@"Song-Files/{timingsFileName}");
        Debug.Log("trying to load timing: " + timingsFileName);
        if (timingTextAsset == null)
        {
            Debug.LogError("Failed to load timing text file.");
            return;
        }
        else{
            Debug.Log("Read the file!");
        }

        // Parse the text asset to extract spawn times and note types
        string[] lines = timingTextAsset.text.Split('\n');
         if (lines.Length < 1)
        {
            Debug.LogError("No BPM found in the timing text file.");
            return;
        }
        if (!float.TryParse(lines[0], out float bpm))
        {
            Debug.LogError("Failed to parse BPM from the timing text file.");
            return;
        }
        else{
            songBPM = bpm;
        }
        
        musicFileName = lines[1].Trim();

        spawnTimes = new float[lines.Length-3];
        noteTypes = new string[lines.Length-3];

        for (int i = 3; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length >= 2 && float.TryParse(parts[0], out float timing))
            {
                spawnTimes[i-3] = timing;
                noteTypes[i-3] = parts[1].Trim(); // Remove any leading or trailing whitespace
            }
            else
            {
                Debug.LogError("Failed to parse timing: " + lines[i]);
            }
        }

        // Load the music
        AudioClip musicAsset = Resources.Load<AudioClip>($"music/{musicFileName}");
        if (musicAsset != null) {
            audioSource.clip = musicAsset;
        } else {
            Debug.LogError($"Unable to open music file {musicFileName}");
        }

        currentSpawnIndex = 0;
        elapsedTime = 0;
    }

    void Update()
    {
        if (hasStarted && spawnTimes != null)
        {
            // Check if all notes have been spawned
            if (currentSpawnIndex >= spawnTimes.Length)
                return;

            // Increment timer
            elapsedTime += Time.deltaTime;

            // Check if it's time to spawn the next note
            if (elapsedTime >= spawnTimes[currentSpawnIndex])
            {
                SpawnNote();
                currentSpawnIndex++;
            }
        }
    }

    bool SkipNote() {
        switch (difficulty) {
            case Difficulty.HARD:
                return false;
            case Difficulty.MEDIUM:
                return noteSkip%2 == 0; //Skips every other note
            case Difficulty.EASY:
                return noteSkip%3 > 0; //Skips every 2 notes
        }

        return false;
    }

    void SpawnNote()
    {
        if (notePrefabs.Length == 0)
        {
            Debug.LogWarning("No note prefabs assigned to the NoteSpawner.");
            return;
        }

        // Find the index of the note prefab corresponding to the note type
        int prefabIndex = -1;        
        for (int i = 0; i < notePrefabs.Length; i++)
        {
            if (notePrefabs[i].name == noteTypes[currentSpawnIndex])
            {
                prefabIndex = i;
                break;
            }
        }

        if (prefabIndex == -1)
        {
            Debug.LogError("Failed to find prefab for note type: " + noteTypes[currentSpawnIndex]);
            return;
        }

        noteSkip += 1;
        if (SkipNote()) return;

        // Spawn the selected note prefab at the spawn point
        GameObject newNote = Instantiate(notePrefabs[prefabIndex], GameManager.instance.buttonSpawnLocation);
        newNote.GetComponent<NoteObject>().ns = this;
        spawnedNotes.Add(newNote.GetComponent<NoteObject>());
        ButtonController.ChangeArrowMode(musicGameMode, notePrefabs[prefabIndex].name, newNote.transform, 0f);

        // Attach the BeatScroller script to the instantiated note GameObject
        BeatScroller beatScroller = newNote.AddComponent<BeatScroller>();
        beatScroller.hasStarted = true; // Start scrolling immediately
        beatScroller.beatTempo = songBPM;
    }

    public void LoadSongFile(string timingsFile) {
        timingsFileName = timingsFile.Trim();
        Debug.Log("notespawner: " + timingsFileName);
        LoadSpawnTimes();
    }

    public void RemoveNoteFromList(NoteObject note) {
        spawnedNotes.Remove(note);
    }

    public void DestroyAllNotes() {
        for (int i = 0; i < spawnedNotes.Count; i++) {
            if (spawnedNotes[i] != null) {
                Destroy(spawnedNotes[i].gameObject);
            }
        }
    }

    public AudioSource GetAudioSource() {
        return audioSource;
    }
}

public enum Difficulty {
    EASY = 0,
    MEDIUM,
    HARD
}

public enum GameMode {
    MIX = 0,
    DDR,
    PIU
}