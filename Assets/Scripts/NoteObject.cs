using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public ButtonMap keyToPress;
    public GameObject hitEffect, goodEffect, perfectEffect, missEffect;
    public NoteSpawner ns = null;
    readonly float heightThreshold = 11; //Notes above this y position will get destroyed
    float heightOffset = 7f;

    // Update is called once per frame
    void Update()
    {
        List<KeyCode> keys = GameManager.GetKeyVal();

        foreach (KeyCode key in keys) {
            if (SerialController.SerialPortIsActive()) {
                // This is for the DDR board
                if (key == (KeyCode)keyToPress) {
                    CheckForNoteHit();
                }
            }
        }
        
        if (Input.GetKeyDown((KeyCode)keyToPress)) {
            CheckForNoteHit();
        }

        //This cleans up notes that get too far off the screen
        if (transform.position.y > heightThreshold) {
            DestroyNote();
        }
    }

    void CheckForNoteHit() {
        GameObject createdObj;
        Vector3 spawnPosition;

        if(canBePressed){
            if(Mathf.Abs(transform.localPosition.y - heightOffset) > 0.3){
                Debug.Log("normal hit");
                GameManager.instance.NormalHit();
                spawnPosition = new Vector3(0.26f, 3.0f, 0.0f);
                createdObj = Instantiate(hitEffect, GameManager.instance.buttonSpawnLocation);
            }
            else if(Mathf.Abs(transform.localPosition.y - heightOffset) > 0.15){
                Debug.Log("good hit");
                GameManager.instance.GoodHit();
                spawnPosition = new Vector3(0.44f, 3.0f, 0.0f);
                createdObj = Instantiate(goodEffect, GameManager.instance.buttonSpawnLocation);
            }
            else{
                Debug.Log("perfect hit");
                GameManager.instance.PerfectHit();
                spawnPosition = new Vector3(0.3f, 3.0f, 0.0f);
                createdObj = Instantiate(perfectEffect, GameManager.instance.buttonSpawnLocation);
            }
            createdObj.transform.localPosition = spawnPosition;

            gameObject.SetActive(false); //We have to set it to inactive or it will count as a miss
            DestroyNote();
        }
    }

    public void DestroyNote() {
        if (ns == null) {
            Debug.LogWarning("Unable to remove NoteObject from notespawner!");
        } else {
            ns.RemoveNoteFromList(this);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Activator"){
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collilsion) {
        if(collilsion.tag == "Activator" && gameObject.activeSelf){
            canBePressed = false;
            GameManager.instance.NoteMissed();
            Vector3 position = new Vector3(0.3f, 3.0f, 0.0f);
            GameObject miss = Instantiate(missEffect, GameManager.instance.buttonSpawnLocation);
            miss.transform.localPosition = position;
        }
    }
}
