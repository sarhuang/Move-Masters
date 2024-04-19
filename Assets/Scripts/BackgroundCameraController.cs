using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCameraController : MonoBehaviour
{
    int beatChangeFrequency = 8; //Change position on this beat
    public List<Transform> cameraStillLocations;
    double timeSinceLastChange = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange > beatChangeFrequency) {
            timeSinceLastChange = 0;
            PickRandomAnimation();
        }
    }

    void PickRandomAnimation() {
        //TODO: Make it pick between a still or moving animation
        PlayRandomStillLocation();
    }

    void PlayRandomStillLocation() {
        Transform t = GetRandomElement(cameraStillLocations);
        transform.position = t.position;
        transform.rotation = t.rotation;
    }

    T GetRandomElement<T>(List<T> values) {
        int index = Random.Range(0, values.Count);
        T element = values[index];
        return element;
    }
}
