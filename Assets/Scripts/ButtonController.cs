using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using Unity.VisualScripting;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;

   
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode key = GameManager.GetKeyVal();
        Debug.Log("Button controller key: " + key);
        if(Input.GetKeyDown(key)){
            spriteRenderer.sprite = pressedImage;
        }    
        if(Input.GetKeyUp(key)){
            spriteRenderer.sprite = defaultImage;
        }
            //string dataFromArduinoString = serialPort.ReadLine();
            //Debug.Log("Data from Arduino: " + dataFromArduinoString);
        
        
        //Debug.Log(dataFromArduinoString);
        
        /*int keyCodeValue;
        
        if (int.TryParse(dataFromArduinoString, out keyCodeValue))
        {
            KeyCode keyToPress = (KeyCode)keyCodeValue;
            Debug.Log(keyToPress);
            if(Input.GetKeyDown(keyToPress)){
                spriteRenderer.sprite = pressedImage;
            }    
            if(Input.GetKeyUp(keyToPress)){
                spriteRenderer.sprite = defaultImage;
            }
        }
        else{
            Debug.LogError("Failed to parse Arduino data to KeyCode.");
        }*/
    }
}
