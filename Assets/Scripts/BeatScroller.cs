using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;
    public bool hasStarted;
    public float scrollSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        beatTempo = beatTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasStarted){
            transform.localPosition += new Vector3(0f, beatTempo * scrollSpeed * Time.deltaTime, 0);
        }
    }
}
