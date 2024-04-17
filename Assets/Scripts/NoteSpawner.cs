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
    AudioSource audioSource;
    string musicFileName;
    public string musicGameMode;

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
        LoadSpawnTimes();
    }

    void LoadSpawnTimes()
    {
        // Load the text file containing timings from Resources folder
        TextAsset timingTextAsset = Resources.Load<TextAsset>($@"song-files/{timingsFileName}");
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
        musicGameMode = lines[2].Trim();

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

        // Spawn the selected note prefab at the spawn point
        GameObject newNote = Instantiate(notePrefabs[prefabIndex]);
        ButtonController.ChangeArrowMode(musicGameMode, notePrefabs[prefabIndex].name, newNote.transform, 0f);

        // Attach the BeatScroller script to the instantiated note GameObject
        BeatScroller beatScroller = newNote.AddComponent<BeatScroller>();
        beatScroller.hasStarted = true; // Start scrolling immediately
        beatScroller.beatTempo = songBPM;
    }

    public void LoadSongFile(string timingsFile) {
        timingsFileName = timingsFile.Trim();
        LoadSpawnTimes();
    }

    public AudioSource GetAudioSource() {
        return audioSource;
    }
}
