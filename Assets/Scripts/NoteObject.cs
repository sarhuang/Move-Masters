using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public KeyCode keyToPress;
    public GameObject hitEffect, goodEffect, perfectEffect, missEffect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        KeyCode key = GameManager.GetKeyVal();
        //CHANGE THIS LINE FOR THE BUTTON
        if(Input.GetKeyDown(keyToPress)){
        //if(key == keyToPress){ 
            if(canBePressed){
                gameObject.SetActive(false);
                if(Mathf.Abs(transform.position.y - 7) > 0.3){
                    Debug.Log("normal hit");
                    GameManager.instance.NormalHit();
                    Vector3 position = new Vector3(0.26f, 3.0f, 0.0f);
                    Instantiate(hitEffect, position, hitEffect.transform.rotation);
                }
                else if(Mathf.Abs(transform.position.y - 7) > 0.15){
                    Debug.Log("good hit");
                    GameManager.instance.GoodHit();
                    Vector3 position = new Vector3(0.44f, 3.0f, 0.0f);
                    Instantiate(goodEffect, position, goodEffect.transform.rotation);
                }
                else{
                    Debug.Log("perfect hit");
                    GameManager.instance.PerfectHit();
                    Vector3 position = new Vector3(0.3f, 3.0f, 0.0f);
                    Instantiate(perfectEffect, position, perfectEffect.transform.rotation);
                }
            }
        }
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
            Instantiate(missEffect, position, missEffect.transform.rotation);
        }
    }
}
