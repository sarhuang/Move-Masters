using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using Unity.VisualScripting;
using System.Linq;
using System;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public ButtonMap keyToPress;
    static string[] ddrArrows = new string[4]{"Left_Arrow", "Down_Arrow", "Up_Arrow", "Right_Arrow"};
    static string[] piuArrows = new string[5]{"Bottom_Left_Arrow", "Top_Left_Arrow", "Center_Button", "Top_Right_Arrow", "Bottom_Right_Arrow"};
    static float zPos = 8;
    //string[] mixArrows = new string[9]{"Bottom_Left_Arrow", "Top_Left_Arrow", "Left_Arrow", "Down_Arrow", "Center Button", "Up_Arrow", "Right_Arrow", "Top_Right_Arrow", "Bottom_Left_Arrow"};
   
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //ChangeArrowMode(NoteSpawner.musicGameMode, spriteRenderer.name, transform, 7f);
    }

    // Update is called once per frame
    void Update()
    {
        List<KeyCode> keys = GameManager.GetKeyVal();

        foreach (KeyCode key in keys) {
            if (GameManager.instance.SerialPortIsActive()) {
                //This is for the DDR board
                if (key == (KeyCode)keyToPress) {
                    Debug.Log(key + " pressed down");
                    spriteRenderer.sprite = pressedImage;
                } else if (key != (KeyCode)keyToPress) {
                    Debug.Log(key + " key off");
                    spriteRenderer.sprite = defaultImage;
                }
            }
        }

        //This is for playing with Keyboard and Mouse
        if (Input.GetKeyDown((KeyCode)keyToPress)) {
            Debug.Log(keyToPress + " pressed down");
            spriteRenderer.sprite = pressedImage;
        } else if (Input.GetKeyUp((KeyCode)keyToPress)) {
            Debug.Log(keyToPress + " key off");
            spriteRenderer.sprite = defaultImage;
        }
    }

    public static void ChangeArrowMode(string mode, string name, Transform t, float yaxis) {
        if(mode == "ddr"){
            if(name == ddrArrows[0]){
                t.localPosition = new Vector3(-8f, yaxis, zPos);
            }
            else if(name == ddrArrows[1]){
                t.localPosition = new Vector3(-6f, yaxis, zPos);
            }
            else if(name == ddrArrows[2]){
                t.localPosition = new Vector3(-4f, yaxis, zPos);
            }
            else if(name == ddrArrows[3]){
                t.localPosition = new Vector3(-2f, yaxis, zPos);
            }
            else{
                Destroy(t.gameObject);
            }
        }
        else if(mode == "piu"){
            if(name == piuArrows[0]){
                t.localPosition = new Vector3(-4f, yaxis, zPos);
            }
            else if(name == piuArrows[1]){
                t.localPosition = new Vector3(-2, yaxis, zPos);
            }
            else if(name == piuArrows[2]){
                t.localPosition = new Vector3(0, yaxis, zPos);
            }
            else if(name == piuArrows[3]){
                t.localPosition = new Vector3(2f, yaxis, zPos);
            }
            else if(name == piuArrows[4]){
                t.localPosition = new Vector3(4f, yaxis, zPos);
            }
            else{
                Destroy(t.gameObject);
            }
        }
    }
}
